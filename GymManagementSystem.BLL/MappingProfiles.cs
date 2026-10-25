using AutoMapper;
using GymManagementSystem.BLL.ViewModels.BookingsViewModel;
using GymManagementSystem.BLL.ViewModels.MembershipViewModel;
using GymManagementSystem.BLL.ViewModels.MemberViewModels;
using GymManagementSystem.BLL.ViewModels.PlanViewModels;
using GymManagementSystem.BLL.ViewModels.SessionViewModels;
using GymManagementSystem.BLL.ViewModels.TrainerViewModels;
using GymManagementSystem.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            MapMember();
            MapTrainer();
            MapPlan();
            MapSession();
            MapMembership();
            MapBooking();
        }
        private void MapMember()
        {
            CreateMap<Member, MemberViewModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.BuildingNo}, {src.Address.Street}, {src.Address.City}"));

            CreateMap<HealthRecord, HealthRecordViewModel>().ReverseMap();

            CreateMap<CreateMemberViewModel, Member>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address
                {
                    BuildingNo = src.BuildingNumber,
                    Street = src.Street,
                    City = src.City,
                }))
                .ForMember(dest => dest.HealthRecord, opt => opt.MapFrom(src => src.HealthRecordViewModel));

            CreateMap<Member, MemberToBeUpdatedViewModel>()
                .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.BuildingNo))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City));

            CreateMap<MemberToBeUpdatedViewModel, Member>()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Photo, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.Address.BuildingNo = src.BuildingNumber;
                    dest.Address.Street = src.Street;
                    dest.Address.City = src.City;
                });
        }

        private void MapTrainer()
        {
            CreateMap<Trainer, TrainerViewModel>()
               .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.BuildingNo}, {src.Address.Street}, {src.Address.City}"))
               .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Speciality));

            CreateMap<CreateTrainerViewModel, Trainer>()
                .ForMember(dest=>dest.Speciality, opt=>opt.MapFrom(src=>src.Specialties))
                .ForMember(dest=>dest.Address, opt=>opt.MapFrom(src=> new Address
                {
                    BuildingNo = src.BuildingNumber,
                    Street = src.Street,
                    City = src.City,
                }));

            CreateMap<Trainer, TrainerToBeUpdatedViewModel>()
                .ForMember(dest=>dest.Specialties, opt=>opt.MapFrom(src=>src.Speciality))
                .ForMember(dest=>dest.BuildingNumber, opt=>opt.MapFrom(src=>src.Address.BuildingNo))
                .ForMember(dest=>dest.Street, opt=>opt.MapFrom(src=>src.Address.Street))
                .ForMember(dest=>dest.City, opt=>opt.MapFrom(src=>src.Address.City));

            CreateMap<TrainerToBeUpdatedViewModel, Trainer>()
                .ForMember(dest=>dest.Speciality, opt=>opt.MapFrom(src=>src.Specialties))
                .AfterMap((src, dest) =>
                {
                    dest.Address.BuildingNo = src.BuildingNumber;
                    dest.Address.Street = src.Street;
                    dest.Address.City = src.City;
                });
        }

        private void MapPlan()
        {
            CreateMap<Plan, PlanViewModel>()
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.DurationDays));

            CreateMap<Plan, PlanToBeUpdatedViewModel>();

            CreateMap<PlanToBeUpdatedViewModel, Plan>()
                .ForMember(dest => dest.Name, opt => opt.Ignore());
        }

        private void MapSession()
        {
            CreateMap<Session, SessionViewModel>()
                .ForMember(dest=>dest.TrainerName, opt=>opt.MapFrom(src=>src.Trainer.Name))
                .ForMember(dest=>dest.CategoryName, opt=>opt.MapFrom(src=>src.Category.Name));

            CreateMap<CreateSessionViewModel, Session>();

            CreateMap<Trainer,TrainerSelectionViewModel>();
            CreateMap<Category,CategorySelectionViewModel>();

            CreateMap<Session, SessionToBeUpdatedViewModel>().ReverseMap();
        }

        private void MapMembership()
        {
            CreateMap<Membership, MembershipViewModel>()
                .ForMember(dest => dest.MembershipId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member.Name))
                .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.Plan.Name))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<CreateMembershipViewModel, Membership>();

            CreateMap<Member, MemberSelectionViewModel>();

            CreateMap<Plan, PlanSelectionViewModel>();


        }

        private void MapBooking()
        {
            CreateMap<Booking, BookingViewModel>()
                .ForMember(dest=> dest.MemberName,opt=>opt.MapFrom(src=>src.Member.Name))
                .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<CreateBookingViewModel, Booking>();

        }
    }
}
