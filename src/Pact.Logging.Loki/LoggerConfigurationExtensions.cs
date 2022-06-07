using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using Serilog.Sinks.Grafana.Loki;
using Serilog.Sinks.Grafana.Loki.HttpClients;
// ReSharper disable UnusedMember.Global

namespace Pact.Logging.Loki;

public static class LoggerConfigurationExtensions
{
    private const string DefaultOutputTemplate = "{Message:lj}{NewLine}{Exception}";

    /// <summary>
    /// Adds a non-durable sink that will send log events to Grafana Loki.
    /// A non-durable sink will lose data after a system or process restart.
    /// </summary>
    /// <param name="sinkConfiguration">
    /// The logger configuration.
    /// </param>
    /// <param name="uri">
    /// The root URI of Loki.
    /// </param>
    /// <param name="labels">
    /// The global log event labels, which will be user for enriching all requests.
    /// </param>
    /// <param name="filtrationMode">
    /// The mode for labels filtration
    /// <see cref="LokiLabelFiltrationMode"/>
    /// </param>
    /// <param name="filtrationLabels">
    /// The list of label keys used for filtration
    /// </param>
    /// <param name="credentials">
    /// Auth <see cref="LokiCredentials"/>.
    /// </param>
    /// <param name="outputTemplate">
    /// A message template describing the format used to write to the sink.
    /// Default value is <code>"{Message:lj}{NewLine}{Exception}"</code>.
    /// </param>
    /// <param name="restrictedToMinimumLevel">
    /// The minimum level for events passed through the sink.
    /// Default value is <see cref="LevelAlias.Minimum"/>.
    /// </param>
    /// <param name="batchPostingLimit">
    /// The maximum number of events to post in a single batch. Default value is 1000.
    /// </param>
    /// <param name="queueLimit">
    /// The maximum number of events stored in the queue in memory, waiting to be posted over
    /// the network. Default value is infinitely.
    /// </param>
    /// <param name="period">
    /// The time to wait between checking for event batches. Default value is 2 seconds.
    /// </param>
    /// <param name="textFormatter">
    /// The formatter rendering individual log events into text, for example JSON. Default
    /// value is <see cref="MessageTemplateTextFormatter"/>.
    /// </param>
    /// <param name="createLevelLabel">
    /// Should level label be created. Default value is true
    /// The level label always won't be created while using <see cref="ILabelAwareTextFormatter"/>
    /// </param>
    /// <param name="useInternalTimestamp">
    /// Should use internal sink timestamp instead of application one to use as log timestamp.
    /// </param>
    /// <param name="certificatePath">
    /// Path to a PEM-format certificate file to use for client authentication
    /// </param>
    /// <param name="certificateKeyPath">
    /// Path to a PEM-format key file to use for client authentication
    /// </param>
    /// <returns>Logger configuration, allowing configuration to continue.</returns>
    public static LoggerConfiguration GrafanaLokiExtended(
        this LoggerSinkConfiguration sinkConfiguration,
        string uri,
        IEnumerable<LokiLabel>? labels = null,
        LokiLabelFiltrationMode? filtrationMode = null,
        IEnumerable<string>? filtrationLabels = null,
        LokiCredentials? credentials = null,
        string outputTemplate = DefaultOutputTemplate,
        LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
        int batchPostingLimit = 1000,
        int? queueLimit = null,
        TimeSpan? period = null,
        ITextFormatter? textFormatter = null,
        bool createLevelLabel = true,
        bool useInternalTimestamp = false,
        string certificatePath = null,
        string certificateKeyPath = null)
    {
        HttpClient httpClient;
        if (!string.IsNullOrWhiteSpace(certificatePath))
        {
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                SslProtocols = SslProtocols.Tls12
            };
            handler.ClientCertificates.Add(X509Certificate2.CreateFromPemFile(certificatePath, certificateKeyPath));
            httpClient = new HttpClient(handler);
        }
        else
        {
            httpClient = new HttpClient();
        }

        var lokiHttpClient = new LokiHttpClient(httpClient);

        return sinkConfiguration.GrafanaLoki(uri,
            labels,
            filtrationMode,
            filtrationLabels,
            credentials,
            outputTemplate,
            restrictedToMinimumLevel,
            batchPostingLimit,
            queueLimit,
            period,
            textFormatter,
            lokiHttpClient,
            createLevelLabel,
            useInternalTimestamp);
    }
}