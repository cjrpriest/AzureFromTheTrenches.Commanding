﻿using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding
{
    /// <summary>
    /// Options for configuring the command system
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Sets defaults
        /// </summary>
        public Options()
        {
            CommandHandlerContainerRegistration = null;
            CommandHandlerFactoryFunc = null;
            CommandExecutionExceptionHandler = null;
            Reset = false;
            Enrichers = null;
        }

        /// <summary>
        /// Can be used to register a type that implements the ICommandExecutionExceptionHandler interface that will be
        /// invoked in the event of an execution error
        /// </summary>
        public Type CommandExecutionExceptionHandler { get; set; }

        /// <summary>
        /// Unless an alternative implementation of ICommandHandlerFactory is supplied or the CommandHandlerFactoryFunc in this options
        /// class is set then actors are created through the dependency resolver but not all IoC containers can resolve unregistered concrete
        /// types (for example the built in ASP.Net Core IServiceCollection and IServiceProvider IoC cannot). Where this is the case
        /// supply an implementation for the CommandHandlerContainerRegistration action that registers the actors in the container. For
        /// example using an IServiceCollection instance of serviceCollection:
        ///     var options = new Options() { CommandHandlerContainerRegistration = type => services.AddTransient(type, type) };
        /// </summary>
        public Action<Type> CommandHandlerContainerRegistration { get; set; }
        /// <summary>
        /// By default actors are created through the dependency resolver but if a function is assigned to the CommandHandlerFactoryFunc
        /// property then that function will be called to instantiate an handler.
        /// </summary>
        public Func<Type, object> CommandHandlerFactoryFunc { get; set; }
        /// <summary>
        /// The commanding system maintains command registrations, command context enrichers, and auditors between calls to UseCommanding
        /// to enable sub-modules to extend the system without any awareness / tight coupling between them. If you want to force a reset
        /// of the shared state then set this property to true (generally this is only useful in sample apps where you might repeatedly
        /// recreate and reconfigure an IoC container as the included samples do.
        /// </summary>
        public bool Reset { get; set; }
        /// <summary>
        /// It can be useful to include additional properties in the command context, for example the Application Insights Operation ID.
        /// This can be done by setting this property to a set of enrichment objects - these are called in sequence and
        /// are passed the current state of the command contexts enriched properties and can return a dictionary of properties to insert.
        /// If the returned dictionary contains a property that already exists it will be replaced in the command context property bag.
        /// </summary>
        public IEnumerable<ICommandDispatchContextEnricher> Enrichers { get; set; }
    }
}