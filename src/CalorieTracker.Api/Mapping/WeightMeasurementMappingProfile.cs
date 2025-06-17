// Plik WeightMeasurementMappingProfile.cs - konfiguracja mapowania obiektów dla modułu pomiarów wagi.
// Odpowiada za transformację między modelami API, komendami a encjami domenowymi.
using AutoMapper;
using CalorieTracker.Api.Models.WeightMeasurements;
using CalorieTracker.Application.WeightMeasurements.Commands;
using CalorieTracker.Application.WeightMeasurements.Queries;
using CalorieTracker.Domain.Entities;

namespace CalorieTracker.Api.Mapping
{
	/// <summary>
	/// Klasa profilu AutoMappera dla operacji związanych z pomiarami masy ciała.
	/// Definiuje mapowania między modelami API a obiektami komend i encji domenowych.
	/// Obsługuje pomijanie pól ustawianych w kontrolerze oraz kalkulowanych osobno.
	/// </summary>
	public class WeightMeasurementMappingProfile : Profile
	{
		/// <summary>
		/// Inicjalizuje nową instancję profilu mapowania dla pomiarów wagi.
		/// Konfiguruje mapowania dla:
		/// - CreateWeightMeasurementRequest -> CreateWeightMeasurementCommand (pomija UserId)
		/// - UpdateWeightMeasurementRequest -> UpdateWeightMeasurementCommand (pomija Id i UserId)
		/// - WeightMeasurement -> WeightMeasurementDto (pomija ProgressToGoal)
		/// </summary>
		public WeightMeasurementMappingProfile()
		{
			ConfigureCreateMeasurementMapping();
			ConfigureUpdateMeasurementMapping();
			ConfigureMeasurementToDto();
		}

		/// <summary>
		/// Konfiguruje mapowanie z CreateWeightMeasurementRequest na CreateWeightMeasurementCommand.
		/// Pomija pole UserId, które jest ustawiane w kontrolerze na podstawie bieżącego użytkownika.
		/// </summary>
		private void ConfigureCreateMeasurementMapping()
		{
			CreateMap<CreateWeightMeasurementRequest, CreateWeightMeasurementCommand>()
				.ForMember(dest => dest.UserId, opt => opt.Ignore());
		}

		/// <summary>
		/// Konfiguruje mapowanie z UpdateWeightMeasurementRequest na UpdateWeightMeasurementCommand.
		/// Pomija pola Id i UserId, które są ustawiane w kontrolerze.
		/// </summary>
		private void ConfigureUpdateMeasurementMapping()
		{
			CreateMap<UpdateWeightMeasurementRequest, UpdateWeightMeasurementCommand>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.UserId, opt => opt.Ignore());
		}

		/// <summary>
		/// Konfiguruje mapowanie z encji WeightMeasurement na model WeightMeasurementDto.
		/// Pomija pole ProgressToGoal, które jest obliczane osobno w kontrolerze.
		/// </summary>
		private void ConfigureMeasurementToDto()
		{
			CreateMap<WeightMeasurement, WeightMeasurementDto>()
				.ForMember(dest => dest.ProgressToGoal, opt => opt.Ignore());
		}
	}
}