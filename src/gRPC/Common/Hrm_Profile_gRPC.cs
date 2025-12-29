using Contracts;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common;


public interface IHrmProfileClient
{
    Task<HrmProResCode> GetEmpCode(string id, CancellationToken ct = default);


}


public class HrmProfileClient : IHrmProfileClient
{
    private readonly string _servUrl;

    public HrmProfileClient(IConfiguration config)
    {
        _servUrl = config["HrmProUrl"] ?? throw new InvalidOperationException("HRM Profile Service Address not configured");
    }


    public async Task<HrmProResCode> GetEmpCode(string id, CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new HrmProfileService.HrmProfileServiceClient(channel);
        var req = new HrmProRqst { Id = id };
        return await client.GetEmpCodeAsync(req);
    }

    
}
