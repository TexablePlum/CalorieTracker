using AutoMapper;
using CalorieTracker.Api.Models.WeightMeasurements;
using CalorieTracker.Application.WeightMeasurements.Commands;
using CalorieTracker.Application.WeightMeasurements.Queries;
using CalorieTracker.Domain.Entities;

namespace CalorieTracker.Api.Mapping
{
	/// <summary>
	/// Profil AutoMapper dla mapowania obiektów związanych z pomiarami masy ciała
	/// </summary>
	public class WeightMeasurementMappingProfile : Profile
	{
		public WeightMeasurementMappingProfile()
		{
			// Request -> Command mappings
			CreateMap<CreateWeightMeasurementRequest, CreateWeightMeasurementCommand>()
				.ForMember(dest => dest.UserId, opt => opt.Ignore()); // Ustawiamy w kontrolerze

			CreateMap<UpdateWeightMeasurementRequest, UpdateWeightMeasurementCommand>()
				.ForMember(dest => dest.Id, opt => opt.Ignore()) // Ustawiamy w kontrolerze
				.ForMember(dest => dest.UserId, opt => opt.Ignore()); // Ustawiamy w kontrolerze

			// Entity -> DTO mappings
			CreateMap<WeightMeasurement, WeightMeasurementDto>()
				.ForMember(dest => dest.ProgressToGoal, opt => opt.Ignore()); // Kalkulujemy w kontrolerze
		}
	}
}