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
	/// </summary>
	public class WeightMeasurementMappingProfile : Profile
	{
		/// <summary>
		/// Inicjalizuje nową instancję profilu mapowania dla pomiarów wagi.
		/// Konfiguruje wszystkie mapowania wymagane do przetwarzania operacji na pomiarach.
		/// </summary>
		public WeightMeasurementMappingProfile()
		{
			/// <summary>
			/// Mapowanie z <see cref="CreateWeightMeasurementRequest"/> na <see cref="CreateWeightMeasurementCommand"/>.
			/// Transformuje model żądania utworzenia pomiaru na odpowiednią komendę.
			/// Pomija pole UserId, które jest ustawiane w kontrolerze na podstawie bieżącego użytkownika.
			/// </summary>
			CreateMap<CreateWeightMeasurementRequest, CreateWeightMeasurementCommand>()
				.ForMember(dest => dest.UserId, opt => opt.Ignore());

			/// <summary>
			/// Mapowanie z <see cref="UpdateWeightMeasurementRequest"/> na <see cref="UpdateWeightMeasurementCommand"/>.
			/// Transformuje model żądania aktualizacji pomiaru na odpowiednią komendę.
			/// Pomija pola Id i UserId, które są ustawiane w kontrolerze.
			/// </summary>
			CreateMap<UpdateWeightMeasurementRequest, UpdateWeightMeasurementCommand>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.UserId, opt => opt.Ignore());

			/// <summary>
			/// Mapowanie z encji <see cref="WeightMeasurement"/> na model <see cref="WeightMeasurementDto"/>.
			/// Transformuje dane pomiaru wagi na format odpowiedzi API.
			/// Pomija pole ProgressToGoal, które jest obliczane osobno w kontrolerze.
			/// </summary>
			CreateMap<WeightMeasurement, WeightMeasurementDto>()
				.ForMember(dest => dest.ProgressToGoal, opt => opt.Ignore());
		}
	}
}