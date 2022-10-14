﻿using System.Text;
using System.Text.Json;
using core;
using core.models;
using Microsoft.Extensions.Logging;

namespace ses;

public class SesTransformer : ITransformer
{
	private readonly ILogger<SesTransformer> _logger;
	public string SourceState => "ReceivedFromSqs";
	private string DestinationState => "ParsedFromSes";

	public SesTransformer(ILogger<SesTransformer> logger)
	{
		_logger = logger;
	}
	public Task Transform(BankItem item)
	{
		if (item.RawPayload == null)
		{
			return Task.CompletedTask;
		}

		ErrorCatching.ExecuteWithErrorCatching(
			_logger, () =>
			{
				var ses = JsonSerializer.Deserialize<SesJson>(item.RawPayload);
				if (ses?.Message == null)
				{
					_logger.LogError($"Unexpected {nameof(BankItem.RawPayload)} in {item.Id}");
					return;
				}
				var message = JsonSerializer.Deserialize<SesMessageJson>(ses.Message);

				if (message?.Content == null)
				{
					_logger.LogError($"Unexpected {nameof(BankItem.RawPayload)} in {item.Id}");
					return;
				}
				
				item.RawEmail = Encoding.UTF8.GetString(Convert.FromBase64String(message.Content));
				item.State = DestinationState;
			}
		);

		return Task.CompletedTask;
	}
}