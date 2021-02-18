using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Craftsman.Domain.Interfaces.ICustomer;
using Craftsman.Domain.Interfaces.IGateway;
using Craftsman.Domain.Interfaces.Repository;
using Craftsman.Domain.Models;
using Craftsman.Shared.Bases;
using Craftsman.Shared.Commands;
using Craftsman.Shared.Constants;
using Craftsman.Shared.Interfaces;
using Craftsman.Shared.ValueObjects;
using OneOf;

namespace Craftsman.Domain.Handlers.CustomerUseCases
{
    public sealed class CreateCustomer : ICreateCustomerService
    {
        private readonly INotifications _notification;
        private readonly IZipCodeServices _zipCodeServices;
        private readonly ICustomerRepository _customerRepository;

        public CreateCustomer(INotifications notifications,
                              IZipCodeServices zipCodeServices,
                              ICustomerRepository customerRepository)
        {
            _notification = notifications;
            _zipCodeServices = zipCodeServices;
            _customerRepository = customerRepository;
        }

        public async Task
                    <OneOf<IReadOnlyCollection<Notification>,
                    Customer,
                    Exception>> Execute(NewCustomerCommand command)
        {
            try
            {
                var domain = BuildCustomerDomain(command);

                if (!domain.IsValid)
                    AddNotification(domain.Notifications);

                if (await SomeDocument(domain.Cpf).ConfigureAwait(false))
                    AddNotification(PropertyName.CPF, Message.CustomerAlreadyExistWithThisCpf);

                if (!await ZipCodeEligible(domain.Address.ZipCode).ConfigureAwait(false))
                    AddNotification(PropertyName.ZipCode, Message.ValueNotExistingInTheBrazilianTerritory);

                await PersistCustomerDataInTheDatabase(domain).ConfigureAwait(false);

                return HasNotifications()
                        ? Notifications().ToList()
                        : domain;
            }
            catch (Exception exception)
            {
                RollBackTransaction();
                return exception;
            }
        }

        private async Task PersistCustomerDataInTheDatabase(Customer model)
        {
            if (HasNotifications()) return;

            BeginTransaction();
            await _customerRepository.Save(model).ConfigureAwait(false);
            CommitTransaction();
        }

        private bool HasNotifications() => _notification.HasNotifications();
        private IReadOnlyCollection<Notification> Notifications() => _notification.GetNotifications();
        private void AddNotification(string property, string message) => _notification.AddNotification(property, message);
        private void AddNotification(List<Notification> notifications) => _notification.AddNotification(notifications);
        private Task<bool> SomeDocument(Cpf value) => _customerRepository.CheckIfCustomerAlreadyExistsByCpf(value);
        private Task<bool> ZipCodeEligible(ZipCode value) => _zipCodeServices.ExistsInBrazil(value.ToString());
        private void BeginTransaction() => _customerRepository.BeginTransaction();
        private void CommitTransaction() => _customerRepository.Commit();
        private void RollBackTransaction() => _customerRepository.Rollback();
        private static Customer BuildCustomerDomain(NewCustomerCommand input)
        => new(input.Name,input.FullName,input.Cpf,input.Email,input.BirthDate,input.Street,input.ZipCode,input.City,input.Country);
    }
}