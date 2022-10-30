using System.Net.Http.Headers;
using System.Text.Json;
using core.Configuration;
using firefly_iii.models;
using Microsoft.Extensions.Logging;

namespace firefly_iii;

public class FireflyClient
{
	private readonly IEnvironmentVariableConfiguration _environmentVariableConfiguration;
	private readonly ILogger<FireflyClient> _logger;
	private readonly HttpClient _httpClient;

	public FireflyClient
	(
		IEnvironmentVariableConfiguration environmentVariableConfiguration,
		ILogger<FireflyClient> logger
	)
	{
		_environmentVariableConfiguration = environmentVariableConfiguration;
		_logger = logger;
		_httpClient = new HttpClient();

		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", environmentVariableConfiguration.FireflyToken);
	}

	public async Task<IEnumerable<Budget>> GetBudgets()
	{
		var currentDate = DateTimeOffset.Now;
		var firstDayOfMonth = new DateTimeOffset(currentDate.Year, currentDate.Month, 1, 0, 0, 0, currentDate.Offset);
		var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
		var requestUrl = $"{_environmentVariableConfiguration.FireflyHost}/api/v1/budgets?page=1&start={firstDayOfMonth.ToString("d")}&end={lastDayOfMonth.ToString("d")}";
		var response = await _httpClient.GetAsync(requestUrl);

		var responseContent = await response.Content.ReadAsStringAsync();		
		if (!response.IsSuccessStatusCode)
		{
			_logger.LogError($"{response.StatusCode} - {response.ReasonPhrase}");
			_logger.LogError(responseContent);
			return Array.Empty<Budget>();
		}

		var budgetResponse = JsonSerializer.Deserialize<ListBudgetsResponse>(responseContent);
		
		// ReSharper disable once InvertIf
		if (budgetResponse == null)
		{
			_logger.LogError(responseContent);
			_logger.LogError("Could not be parsed");
			return Array.Empty<Budget>();
		}
		return budgetResponse.Data;
	}
}