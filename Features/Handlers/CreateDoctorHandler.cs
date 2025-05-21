using AutoMapper;
using MediatR;
using PatientRecovery.Shared.Messaging;
using PatientRecoverySystem.DoctorService.DTOs;
using PatientRecoverySystem.DoctorService.Features.Commands;
using PatientRecoverySystem.DoctorService.Models;
using PatientRecoverySystem.DoctorService.Repositories;
using System.Text.Json;

namespace PatientRecoverySystem.DoctorService.Features.Handlers
{
    public class CreateDoctorHandler : IRequestHandler<CreateDoctorCommand, DoctorDto>
    {
        private readonly IDoctorRepository _repository;
        private readonly IMapper _mapper;
        private readonly IRabbitMQService _messageBus;

        public CreateDoctorHandler(
            IDoctorRepository repository,
            IMapper mapper,
            IRabbitMQService messageBus)
        {
            _repository = repository;
            _mapper = mapper;
            _messageBus = messageBus;
        }

        public async Task<DoctorDto> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
        {
            var doctor = _mapper.Map<Doctor>(request);
            var createdDoctor = await _repository.CreateAsync(doctor);
            
            var doctorDto = _mapper.Map<DoctorDto>(createdDoctor);
            
            // Notify other services about new doctor
            _messageBus.PublishDoctorCreated(JsonSerializer.Serialize(doctorDto));
            
            return doctorDto;
        }
    }
}