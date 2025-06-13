using AutoMapper;
using CalorieTracker.Api.Attributes;
using CalorieTracker.Api.Models.WeightMeasurements;
using CalorieTracker.Application.WeightMeasurements.Commands;
using CalorieTracker.Application.WeightMeasurements.Handlers;
using CalorieTracker.Application.WeightMeasurements.Queries;
using CalorieTracker.Domain.Services;
using CalorieTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CalorieTracker.Api.Controllers
{
	/// <summary>
	/// Kontroler do zarządzania pomiarami masy ciała
	/// </summary>
	[Authorize]
	[RequireCompleteProfile]
	[ApiController]
	[Route("api/[controller]")]
	public class WeightMeasurementsController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly WeightAnalysisService _weightAnalysis;
		private readonly IAppDbContext _db;
		private readonly CreateWeightMeasurementHandler _createHandler;
		private readonly UpdateWeightMeasurementHandler _updateHandler;
		private readonly DeleteWeightMeasurementHandler _deleteHandler;
		private readonly GetUserWeightMeasurementsHandler _getUserMeasurementsHandler;
		private readonly GetLatestWeightMeasurementHandler _getLatestHandler;
		private readonly GetWeightMeasurementDetailsHandler _getDetailsHandler;

		public WeightMeasurementsController(
			IMapper mapper,
			WeightAnalysisService weightAnalysis,
			IAppDbContext db,
			CreateWeightMeasurementHandler createHandler,
			UpdateWeightMeasurementHandler updateHandler,
			DeleteWeightMeasurementHandler deleteHandler,
			GetUserWeightMeasurementsHandler getUserMeasurementsHandler,
			GetLatestWeightMeasurementHandler getLatestHandler,
			GetWeightMeasurementDetailsHandler getDetailsHandler)
		{
			_mapper = mapper;
			_weightAnalysis = weightAnalysis;
			_db = db;
			_createHandler = createHandler;
			_updateHandler = updateHandler;
			_deleteHandler = deleteHandler;
			_getUserMeasurementsHandler = getUserMeasurementsHandler;
			_getLatestHandler = getLatestHandler;
			_getDetailsHandler = getDetailsHandler;
		}

		/// <summary>
		/// Pobiera pomiary masy ciała użytkownika z paginacją
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<WeightMeasurementsResponse>> GetWeightMeasurements([FromQuery] int skip = 0, [FromQuery] int take = 20)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			var query = new GetUserWeightMeasurementsQuery { UserId = userId, Skip = skip, Take = take };
			var measurements = await _getUserMeasurementsHandler.Handle(query);

			// Pobierz profil użytkownika dla kalkulacji progressToGoal
			var userProfile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

			var measurementDtos = new List<WeightMeasurementDto>();
			foreach (var measurement in measurements)
			{
				var dto = _mapper.Map<WeightMeasurementDto>(measurement);

				// Oblicz postęp względem celu
				if (userProfile?.TargetWeightKg != null)
				{
					dto.ProgressToGoal = _weightAnalysis.CalculateProgressToGoal(measurement.WeightKg, userProfile.TargetWeightKg.Value);
				}

				measurementDtos.Add(dto);
			}

			return Ok(new WeightMeasurementsResponse
			{
				Measurements = measurementDtos,
				TotalCount = measurementDtos.Count,
				HasMore = measurementDtos.Count == take
			});
		}

		/// <summary>
		/// Pobiera najnowszy pomiar masy ciała użytkownika
		/// </summary>
		[HttpGet("latest")]
		public async Task<ActionResult<WeightMeasurementDto>> GetLatestWeightMeasurement()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			var measurement = await _getLatestHandler.Handle(new GetLatestWeightMeasurementQuery(userId));
			if (measurement is null) return NotFound("Brak pomiarów masy ciała");

			var userProfile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
			var dto = _mapper.Map<WeightMeasurementDto>(measurement);

			// Oblicz postęp względem celu
			if (userProfile?.TargetWeightKg != null)
			{
				dto.ProgressToGoal = _weightAnalysis.CalculateProgressToGoal(measurement.WeightKg, userProfile.TargetWeightKg.Value);
			}

			return Ok(dto);
		}

		/// <summary>
		/// Pobiera szczegóły konkretnego pomiaru
		/// </summary>
		[HttpGet("{id:guid}")]
		public async Task<ActionResult<WeightMeasurementDto>> GetWeightMeasurement(Guid id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			var measurement = await _getDetailsHandler.Handle(new GetWeightMeasurementDetailsQuery(id, userId));
			if (measurement is null) return NotFound("Pomiar nie został znaleziony");

			var userProfile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
			var dto = _mapper.Map<WeightMeasurementDto>(measurement);

			// Oblicz postęp względem celu
			if (userProfile?.TargetWeightKg != null)
			{
				dto.ProgressToGoal = _weightAnalysis.CalculateProgressToGoal(measurement.WeightKg, userProfile.TargetWeightKg.Value);
			}

			return Ok(dto);
		}

		/// <summary>
		/// Tworzy nowy pomiar masy ciała
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<Guid>> CreateWeightMeasurement([FromBody] CreateWeightMeasurementRequest request)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			var command = _mapper.Map<CreateWeightMeasurementCommand>(request);
			command = command with { UserId = userId };

			try
			{
				var measurementId = await _createHandler.Handle(command);

				return CreatedAtAction(
					nameof(GetWeightMeasurement),
					new { id = measurementId },
					new { id = measurementId });
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Aktualizuje istniejący pomiar masy ciała
		/// </summary>
		[HttpPut("{id:guid}")]
		public async Task<IActionResult> UpdateWeightMeasurement(Guid id, [FromBody] UpdateWeightMeasurementRequest request)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			var command = _mapper.Map<UpdateWeightMeasurementCommand>(request);
			command = command with { Id = id, UserId = userId };

			var success = await _updateHandler.Handle(command);

			if (!success)
				return NotFound("Pomiar nie został znaleziony");

			return NoContent();
		}

		/// <summary>
		/// Usuwa pomiar masy ciała
		/// </summary>
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> DeleteWeightMeasurement(Guid id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			var success = await _deleteHandler.Handle(new DeleteWeightMeasurementCommand(id, userId));

			if (!success)
				return NotFound("Pomiar nie został znaleziony");

			return NoContent();
		}
	}
}