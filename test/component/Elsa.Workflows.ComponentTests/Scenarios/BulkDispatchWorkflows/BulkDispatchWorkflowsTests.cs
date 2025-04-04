using Elsa.Common.Models;
using Elsa.Testing.Shared.Services;
using Elsa.Workflows.ComponentTests.Abstractions;
using Elsa.Workflows.ComponentTests.Fixtures;
using Elsa.Workflows.ComponentTests.Scenarios.BulkDispatchWorkflows.Workflows;
using Elsa.Workflows.Models;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa.Workflows.ComponentTests.Scenarios.BulkDispatchWorkflows;

public class BulkDispatchWorkflowsTests : AppComponentTest
{
    private readonly SignalManager _signalManager;
    private readonly IWorkflowRuntime _workflowRuntime;

    public BulkDispatchWorkflowsTests(App app) : base(app)
    {
        _workflowRuntime = Scope.ServiceProvider.GetRequiredService<IWorkflowRuntime>();
        _signalManager = Scope.ServiceProvider.GetRequiredService<SignalManager>();
    }

    // /// <summary>
    // /// Dispatches and waits for child workflows to complete.
    // /// </summary>
    // [Fact(Skip = "This test is flaky and needs to be fixed.")]
    // public async Task DispatchAndWaitWorkflow_ShouldWaitForChildWorkflowToComplete()
    // {
    //     var workflowClient = await _workflowRuntime.CreateClientAsync();
    //     await workflowClient.CreateInstanceAsync(new CreateWorkflowInstanceRequest
    //     {
    //         WorkflowDefinitionHandle = WorkflowDefinitionHandle.ByDefinitionId(GreetEmployeesWorkflow.DefinitionId, VersionOptions.Published)
    //     });
    //     await workflowClient.RunInstanceAsync(RunWorkflowInstanceRequest.Empty);
    //     await _signalManager.WaitAsync<string>("Completed");
    // }

    /// <summary>
    /// Individual items are sent as input to child workflows.
    /// </summary>
    [Fact]
    public async Task DispatchWorkflows_ChildWorkflowsShouldReceiveCurrentItem()
    {
        var workflowClient = await _workflowRuntime.CreateClientAsync();
        var request = new CreateAndRunWorkflowInstanceRequest
        {
            WorkflowDefinitionHandle = WorkflowDefinitionHandle.ByDefinitionId(MixFruitsWorkflow.DefinitionId, VersionOptions.Published)
        };
        await workflowClient.CreateAndRunInstanceAsync(request);

        await _signalManager.WaitAsync("Apple");
        await _signalManager.WaitAsync("Banana");
        await _signalManager.WaitAsync("Cherry");
    }
}