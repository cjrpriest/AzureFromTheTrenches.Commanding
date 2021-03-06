﻿namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public interface IQueueableCommand
    {
        bool ShouldDequeue { get; set; }

        int DequeueCount { get; set; }
    }
}
