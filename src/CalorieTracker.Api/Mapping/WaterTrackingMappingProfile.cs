using AutoMapper;
using CalorieTracker.Api.Models.NutritionTracking;
using CalorieTracker.Application.NutritionTracking.Commands;
using CalorieTracker.Domain.Entities;

namespace CalorieTracker.Api.Mapping
{
	/// <summary>
	/// Klasa profilu AutoMappera dla operacji związanych ze śledzeniem spożycia wody.
	/// Definiuje mapowania między modelami DTO a komendami oraz encjami domenowymi.
	/// </summary>
	public class WaterTrackingMappingProfile : Profile
	{
		/// <summary>
		/// Inicjalizuje nową instancję profilu mapowania dla water tracking.
		/// Konfiguruje wszystkie mapowania między API DTOs, komendami aplikacji i encjami domeny.
		/// </summary>
		public WaterTrackingMappingProfile()
		{
			ConfigureRequestToCommandMappings();
		}

		/// <summary>
		/// Konfiguruje mapowania z modeli żądań API na komendy aplikacji.
		/// </summary>
		private void ConfigureRequestToCommandMappings()
		{
			// Mapowanie LogWaterIntakeRequest -> LogWaterIntakeCommand
			CreateMap<LogWaterIntakeRequest, LogWaterIntakeCommand>()
				.ForMember(dest => dest.UserId, opt => opt.Ignore()); // UserId ustawiany w kontrolerze

			// Mapowanie UpdateWaterIntakeRequest -> UpdateWaterIntakeCommand
			CreateMap<UpdateWaterIntakeRequest, UpdateWaterIntakeCommand>()
				.ForMember(dest => dest.WaterIntakeLogEntryId, opt => opt.Ignore()) // ID ustawiany w kontrolerze
				.ForMember(dest => dest.UserId, opt => opt.Ignore()); // UserId ustawiany w kontrolerze
		}
	}
}