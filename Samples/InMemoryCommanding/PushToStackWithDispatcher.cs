﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.MicrosoftDependencyInjection;
using InMemoryCommanding.Commands;
using InMemoryCommanding.Results;
using Microsoft.Extensions.DependencyInjection;

namespace InMemoryCommanding
{
    class StackDispatcher : ICommandDispatcher
    {
        private readonly Stack<object> _commandStack;

        public StackDispatcher(Stack<object> commandStack)
        {
            _commandStack = commandStack;
        }

        public Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
        {
            _commandStack.Push(command);
            return Task.FromResult(new CommandResult<TResult>(default(TResult), true));
        }

        public Task<CommandResult> DispatchAsync(ICommand command, CancellationToken cancellationToken)
        {
            _commandStack.Push(command);
            return Task.FromResult(new CommandResult(true));
        }

        public ICommandExecuter AssociatedExecuter => null;
    }

    static class PushToStackWithDispatcher
    {
        public static async Task Run()
        {
            Stack<object> stack = new Stack<object>();
            ICommandDispatcher dispatcher = Configure(stack);
            await dispatcher.DispatchAsync(new OutputToConsoleCommand { Message = "Hello" });
            await dispatcher.DispatchAsync(new OutputToConsoleCommand { Message = "World" });

            while (stack.Any())
            {
                OutputToConsoleCommand command = (OutputToConsoleCommand) stack.Pop();
                Console.WriteLine(command.Message);
            }

            Console.WriteLine("\nPress a key to continue...");
        }

        private static ICommandDispatcher Configure(Stack<object> stack)
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            IMicrosoftDependencyInjectionCommandingResolver dependencyResolver = new MicrosoftDependencyInjectionCommandingResolver(serviceCollection);
            Options options = new Options
            {
                Reset = true // we reset the registry because we allow repeat runs, in a normal app this isn't required                
            };
            dependencyResolver.UseCommanding(options)
                .Register<OutputToConsoleCommand, CountResult>(() => new StackDispatcher(stack));
            dependencyResolver.ServiceProvider = serviceCollection.BuildServiceProvider();
            return dependencyResolver.ServiceProvider.GetService<ICommandDispatcher>();
        }
    }
}
