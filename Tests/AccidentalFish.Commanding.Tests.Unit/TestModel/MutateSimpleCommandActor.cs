﻿using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.TestModel
{
    class MutateSimpleCommandActor : ICommandActor<SimpleCommand, SimpleResult>
    {
        public Task<SimpleResult> ExecuteAsync(SimpleCommand command, SimpleResult previousResult)
        {
            command.Message = "i did mutate it";
            return Task.FromResult(previousResult);
        }
    }
}
