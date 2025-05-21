namespace PatientRecovery.Shared.Messaging
{
    public interface IRabbitMQService
    {
        void PublishMessage(string message, string routingKey);
        void PublishDoctorCreated(string message);
        void PublishDoctorUpdated(string message);
        void PublishConsultationScheduled(string message);
        void PublishConsultationStatusChanged(string message);
        void PublishVitalSignsAlert(string message);
    }
}