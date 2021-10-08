using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GlobusDesktop.Helpers;
using Microsoft.Extensions.Configuration;
using ObservabilityPlatform.GrafanaClient;
using ObservabilityPlatform.GrafanaClient.Entities;
using ObservabilityPlatform.GrafanaClient.Responses;

namespace GlobusDesktop.Services
{
    public class GrafanaService : IGrafanaService
    {
        private readonly IGrafana _grafana;

        public GrafanaService(IConfiguration configuration, IConfigurationService appConfig)
        {
            if (configuration["UseTokenAuthentication"] == "true")
            {
                try
                {
                    var (host, token) = EnvironmentHelper.GetBearerSecrets();

                    _grafana = new Grafana.Builder()
                        .ConnectTo(host)
                        .UseTokenAuthentication(token)
                        .Build();
                }
                //If there are no env vars on this system
                catch (ArgumentNullException e)
                {
                    var (host, token) = (
                        appConfig.App.GrafanaConfiguration.Host, 
                        appConfig.App.GrafanaConfiguration.Token);

                    _grafana = new Grafana.Builder()
                        .ConnectTo(host)
                        .UseTokenAuthentication(token)
                        .Build();
                }
            }
            else
            {
                var (host, login, password) = EnvironmentHelper.GetBasicSecrets();

                _grafana = new Grafana.Builder()
                    .ConnectTo(host)
                    .UseBaseAuthentication(login, password)
                    .Build();
            }
        }

        public async Task<PostDatasourceResult> CreateDatasource(Datasource datasource)
        {
            var response = await _grafana.CreateDataSourceWithBasicAuth(datasource);

            return response;
        }

        public async Task<string> CreateDashboardWithoutValidation(string dashboard)
        {
            var response = await _grafana.CreateDashboardWithoutValidation(dashboard);

            return response;
        }

        public async Task<List<Datasource>> GetAllDatasources()
        {
            var datasources = await _grafana.GetAllDataSources();

            return datasources;
        }

        public async Task<GetDashboardResponse> GetDashboard(string uid)
        {
            var response = await _grafana.GetDashboard(uid);

            return response;
        }
    }
}