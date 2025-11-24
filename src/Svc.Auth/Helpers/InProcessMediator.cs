namespace Svc.Auth.Helpers;

public class InProcessMediator
{
    private readonly IServiceProvider _serviceProvider;

    public InProcessMediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        dynamic handler = _serviceProvider.GetService(handlerType) ?? throw new InvalidOperationException($"Handler for {request.GetType().Name} not registered.");
        return await handler.Handle((dynamic)request);
    }
}