using System;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Microsoft.DotNet.Maestro.Client.Models;

namespace Microsoft.DotNet.Maestro.Client
{
    public partial interface ISubscriptions
    {
        Task<IImmutableList<Subscription>> ListSubscriptionsAsync(
            int? channelId = default,
            bool? enabled = default,
            string sourceRepository = default,
            string targetRepository = default,
            CancellationToken cancellationToken = default
        );

        Task<Subscription> CreateAsync(
            SubscriptionData body,
            CancellationToken cancellationToken = default
        );

        Task<Subscription> GetSubscriptionAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<Subscription> DeleteSubscriptionAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<Subscription> UpdateSubscriptionAsync(
            Guid id,
            SubscriptionUpdate body = default,
            CancellationToken cancellationToken = default
        );

        Task<Subscription> TriggerSubscriptionAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task TriggerDailyUpdateAsync(
            CancellationToken cancellationToken = default
        );

        Task<PagedResponse<SubscriptionHistoryItem>> GetSubscriptionHistoryAsync(
            Guid id,
            int? page = default,
            int? perPage = default,
            CancellationToken cancellationToken = default
        );

        Task RetrySubscriptionActionAsyncAsync(
            Guid id,
            long timestamp,
            CancellationToken cancellationToken = default
        );

    }

    internal partial class Subscriptions : IServiceOperations<MaestroApi>, ISubscriptions
    {
        public Subscriptions(MaestroApi client)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public MaestroApi Client { get; }

        partial void HandleFailedRequest(RestApiException ex);

        partial void HandleFailedListSubscriptionsRequest(RestApiException ex);

        public async Task<IImmutableList<Subscription>> ListSubscriptionsAsync(
            int? channelId = default,
            bool? enabled = default,
            string sourceRepository = default,
            string targetRepository = default,
            CancellationToken cancellationToken = default
        )
        {
            const string apiVersion = "2019-01-16";

            var _baseUri = Client.Options.BaseUri;
            var _url = new RequestUriBuilder();
            _url.Reset(_baseUri);
            _url.AppendPath(
                "/api/subscriptions",
                false);

            if (!string.IsNullOrEmpty(sourceRepository))
            {
                _url.AppendQuery("sourceRepository", Client.Serialize(sourceRepository));
            }
            if (!string.IsNullOrEmpty(targetRepository))
            {
                _url.AppendQuery("targetRepository", Client.Serialize(targetRepository));
            }
            if (channelId != default(int?))
            {
                _url.AppendQuery("channelId", Client.Serialize(channelId));
            }
            if (enabled != default(bool?))
            {
                _url.AppendQuery("enabled", Client.Serialize(enabled));
            }
            _url.AppendQuery("api-version", Client.Serialize(apiVersion));


            using (var _req = Client.Pipeline.CreateRequest())
            {
                _req.Uri = _url;
                _req.Method = RequestMethod.Get;

                using (var _res = await Client.SendAsync(_req, cancellationToken).ConfigureAwait(false))
                {
                    if (_res.Status < 200 || _res.Status >= 300)
                    {
                        await OnListSubscriptionsFailed(_req, _res).ConfigureAwait(false);
                    }

                    if (_res.ContentStream == null)
                    {
                        await OnListSubscriptionsFailed(_req, _res).ConfigureAwait(false);
                    }

                    using (var _reader = new StreamReader(_res.ContentStream))
                    {
                        var _content = await _reader.ReadToEndAsync().ConfigureAwait(false);
                        var _body = Client.Deserialize<IImmutableList<Subscription>>(_content);
                        return _body;
                    }
                }
            }
        }

        internal async Task OnListSubscriptionsFailed(Request req, Response res)
        {
            string content = null;
            if (res.ContentStream != null)
            {
                using (var reader = new StreamReader(res.ContentStream))
                {
                    content = await reader.ReadToEndAsync().ConfigureAwait(false);
                }
            }

            var ex = new RestApiException<ApiError>(
                req,
                res,
                content,
                Client.Deserialize<ApiError>(content)
                );
            HandleFailedListSubscriptionsRequest(ex);
            HandleFailedRequest(ex);
            Client.OnFailedRequest(ex);
            throw ex;
        }

        partial void HandleFailedCreateRequest(RestApiException ex);

        public async Task<Subscription> CreateAsync(
            SubscriptionData body,
            CancellationToken cancellationToken = default
        )
        {
            if (body == default(SubscriptionData))
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (!body.IsValid)
            {
                throw new ArgumentException("The parameter is not valid", nameof(body));
            }

            const string apiVersion = "2019-01-16";

            var _baseUri = Client.Options.BaseUri;
            var _url = new RequestUriBuilder();
            _url.Reset(_baseUri);
            _url.AppendPath(
                "/api/subscriptions",
                false);

            _url.AppendQuery("api-version", Client.Serialize(apiVersion));


            using (var _req = Client.Pipeline.CreateRequest())
            {
                _req.Uri = _url;
                _req.Method = RequestMethod.Post;

                if (body != default(SubscriptionData))
                {
                    _req.Content = RequestContent.Create(Encoding.UTF8.GetBytes(Client.Serialize(body)));
                    _req.Headers.Add("Content-Type", "application/json; charset=utf-8");
                }

                using (var _res = await Client.SendAsync(_req, cancellationToken).ConfigureAwait(false))
                {
                    if (_res.Status < 200 || _res.Status >= 300)
                    {
                        await OnCreateFailed(_req, _res).ConfigureAwait(false);
                    }

                    if (_res.ContentStream == null)
                    {
                        await OnCreateFailed(_req, _res).ConfigureAwait(false);
                    }

                    using (var _reader = new StreamReader(_res.ContentStream))
                    {
                        var _content = await _reader.ReadToEndAsync().ConfigureAwait(false);
                        var _body = Client.Deserialize<Subscription>(_content);
                        return _body;
                    }
                }
            }
        }

        internal async Task OnCreateFailed(Request req, Response res)
        {
            string content = null;
            if (res.ContentStream != null)
            {
                using (var reader = new StreamReader(res.ContentStream))
                {
                    content = await reader.ReadToEndAsync().ConfigureAwait(false);
                }
            }

            var ex = new RestApiException<ApiError>(
                req,
                res,
                content,
                Client.Deserialize<ApiError>(content)
                );
            HandleFailedCreateRequest(ex);
            HandleFailedRequest(ex);
            Client.OnFailedRequest(ex);
            throw ex;
        }

        partial void HandleFailedGetSubscriptionRequest(RestApiException ex);

        public async Task<Subscription> GetSubscriptionAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            if (id == default(Guid))
            {
                throw new ArgumentNullException(nameof(id));
            }

            const string apiVersion = "2019-01-16";

            var _baseUri = Client.Options.BaseUri;
            var _url = new RequestUriBuilder();
            _url.Reset(_baseUri);
            _url.AppendPath(
                "/api/subscriptions/{id}".Replace("{id}", Uri.EscapeDataString(Client.Serialize(id))),
                false);

            _url.AppendQuery("api-version", Client.Serialize(apiVersion));


            using (var _req = Client.Pipeline.CreateRequest())
            {
                _req.Uri = _url;
                _req.Method = RequestMethod.Get;

                using (var _res = await Client.SendAsync(_req, cancellationToken).ConfigureAwait(false))
                {
                    if (_res.Status < 200 || _res.Status >= 300)
                    {
                        await OnGetSubscriptionFailed(_req, _res).ConfigureAwait(false);
                    }

                    if (_res.ContentStream == null)
                    {
                        await OnGetSubscriptionFailed(_req, _res).ConfigureAwait(false);
                    }

                    using (var _reader = new StreamReader(_res.ContentStream))
                    {
                        var _content = await _reader.ReadToEndAsync().ConfigureAwait(false);
                        var _body = Client.Deserialize<Subscription>(_content);
                        return _body;
                    }
                }
            }
        }

        internal async Task OnGetSubscriptionFailed(Request req, Response res)
        {
            string content = null;
            if (res.ContentStream != null)
            {
                using (var reader = new StreamReader(res.ContentStream))
                {
                    content = await reader.ReadToEndAsync().ConfigureAwait(false);
                }
            }

            var ex = new RestApiException<ApiError>(
                req,
                res,
                content,
                Client.Deserialize<ApiError>(content)
                );
            HandleFailedGetSubscriptionRequest(ex);
            HandleFailedRequest(ex);
            Client.OnFailedRequest(ex);
            throw ex;
        }

        partial void HandleFailedDeleteSubscriptionRequest(RestApiException ex);

        public async Task<Subscription> DeleteSubscriptionAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            if (id == default(Guid))
            {
                throw new ArgumentNullException(nameof(id));
            }

            const string apiVersion = "2019-01-16";

            var _baseUri = Client.Options.BaseUri;
            var _url = new RequestUriBuilder();
            _url.Reset(_baseUri);
            _url.AppendPath(
                "/api/subscriptions/{id}".Replace("{id}", Uri.EscapeDataString(Client.Serialize(id))),
                false);

            _url.AppendQuery("api-version", Client.Serialize(apiVersion));


            using (var _req = Client.Pipeline.CreateRequest())
            {
                _req.Uri = _url;
                _req.Method = RequestMethod.Delete;

                using (var _res = await Client.SendAsync(_req, cancellationToken).ConfigureAwait(false))
                {
                    if (_res.Status < 200 || _res.Status >= 300)
                    {
                        await OnDeleteSubscriptionFailed(_req, _res).ConfigureAwait(false);
                    }

                    if (_res.ContentStream == null)
                    {
                        await OnDeleteSubscriptionFailed(_req, _res).ConfigureAwait(false);
                    }

                    using (var _reader = new StreamReader(_res.ContentStream))
                    {
                        var _content = await _reader.ReadToEndAsync().ConfigureAwait(false);
                        var _body = Client.Deserialize<Subscription>(_content);
                        return _body;
                    }
                }
            }
        }

        internal async Task OnDeleteSubscriptionFailed(Request req, Response res)
        {
            string content = null;
            if (res.ContentStream != null)
            {
                using (var reader = new StreamReader(res.ContentStream))
                {
                    content = await reader.ReadToEndAsync().ConfigureAwait(false);
                }
            }

            var ex = new RestApiException<ApiError>(
                req,
                res,
                content,
                Client.Deserialize<ApiError>(content)
                );
            HandleFailedDeleteSubscriptionRequest(ex);
            HandleFailedRequest(ex);
            Client.OnFailedRequest(ex);
            throw ex;
        }

        partial void HandleFailedUpdateSubscriptionRequest(RestApiException ex);

        public async Task<Subscription> UpdateSubscriptionAsync(
            Guid id,
            SubscriptionUpdate body = default,
            CancellationToken cancellationToken = default
        )
        {
            if (id == default(Guid))
            {
                throw new ArgumentNullException(nameof(id));
            }

            const string apiVersion = "2019-01-16";

            var _baseUri = Client.Options.BaseUri;
            var _url = new RequestUriBuilder();
            _url.Reset(_baseUri);
            _url.AppendPath(
                "/api/subscriptions/{id}".Replace("{id}", Uri.EscapeDataString(Client.Serialize(id))),
                false);

            _url.AppendQuery("api-version", Client.Serialize(apiVersion));


            using (var _req = Client.Pipeline.CreateRequest())
            {
                _req.Uri = _url;
                _req.Method = RequestMethod.Patch;

                if (body != default(SubscriptionUpdate))
                {
                    _req.Content = RequestContent.Create(Encoding.UTF8.GetBytes(Client.Serialize(body)));
                    _req.Headers.Add("Content-Type", "application/json; charset=utf-8");
                }

                using (var _res = await Client.SendAsync(_req, cancellationToken).ConfigureAwait(false))
                {
                    if (_res.Status < 200 || _res.Status >= 300)
                    {
                        await OnUpdateSubscriptionFailed(_req, _res).ConfigureAwait(false);
                    }

                    if (_res.ContentStream == null)
                    {
                        await OnUpdateSubscriptionFailed(_req, _res).ConfigureAwait(false);
                    }

                    using (var _reader = new StreamReader(_res.ContentStream))
                    {
                        var _content = await _reader.ReadToEndAsync().ConfigureAwait(false);
                        var _body = Client.Deserialize<Subscription>(_content);
                        return _body;
                    }
                }
            }
        }

        internal async Task OnUpdateSubscriptionFailed(Request req, Response res)
        {
            string content = null;
            if (res.ContentStream != null)
            {
                using (var reader = new StreamReader(res.ContentStream))
                {
                    content = await reader.ReadToEndAsync().ConfigureAwait(false);
                }
            }

            var ex = new RestApiException<ApiError>(
                req,
                res,
                content,
                Client.Deserialize<ApiError>(content)
                );
            HandleFailedUpdateSubscriptionRequest(ex);
            HandleFailedRequest(ex);
            Client.OnFailedRequest(ex);
            throw ex;
        }

        partial void HandleFailedTriggerSubscriptionRequest(RestApiException ex);

        public async Task<Subscription> TriggerSubscriptionAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            if (id == default(Guid))
            {
                throw new ArgumentNullException(nameof(id));
            }

            const string apiVersion = "2019-01-16";

            var _baseUri = Client.Options.BaseUri;
            var _url = new RequestUriBuilder();
            _url.Reset(_baseUri);
            _url.AppendPath(
                "/api/subscriptions/{id}/trigger".Replace("{id}", Uri.EscapeDataString(Client.Serialize(id))),
                false);

            _url.AppendQuery("api-version", Client.Serialize(apiVersion));


            using (var _req = Client.Pipeline.CreateRequest())
            {
                _req.Uri = _url;
                _req.Method = RequestMethod.Post;

                using (var _res = await Client.SendAsync(_req, cancellationToken).ConfigureAwait(false))
                {
                    if (_res.Status < 200 || _res.Status >= 300)
                    {
                        await OnTriggerSubscriptionFailed(_req, _res).ConfigureAwait(false);
                    }

                    if (_res.ContentStream == null)
                    {
                        await OnTriggerSubscriptionFailed(_req, _res).ConfigureAwait(false);
                    }

                    using (var _reader = new StreamReader(_res.ContentStream))
                    {
                        var _content = await _reader.ReadToEndAsync().ConfigureAwait(false);
                        var _body = Client.Deserialize<Subscription>(_content);
                        return _body;
                    }
                }
            }
        }

        internal async Task OnTriggerSubscriptionFailed(Request req, Response res)
        {
            string content = null;
            if (res.ContentStream != null)
            {
                using (var reader = new StreamReader(res.ContentStream))
                {
                    content = await reader.ReadToEndAsync().ConfigureAwait(false);
                }
            }

            var ex = new RestApiException<ApiError>(
                req,
                res,
                content,
                Client.Deserialize<ApiError>(content)
                );
            HandleFailedTriggerSubscriptionRequest(ex);
            HandleFailedRequest(ex);
            Client.OnFailedRequest(ex);
            throw ex;
        }

        partial void HandleFailedTriggerDailyUpdateRequest(RestApiException ex);

        public async Task TriggerDailyUpdateAsync(
            CancellationToken cancellationToken = default
        )
        {
            const string apiVersion = "2019-01-16";

            var _baseUri = Client.Options.BaseUri;
            var _url = new RequestUriBuilder();
            _url.Reset(_baseUri);
            _url.AppendPath(
                "/api/subscriptions/triggerDaily",
                false);

            _url.AppendQuery("api-version", Client.Serialize(apiVersion));


            using (var _req = Client.Pipeline.CreateRequest())
            {
                _req.Uri = _url;
                _req.Method = RequestMethod.Post;

                using (var _res = await Client.SendAsync(_req, cancellationToken).ConfigureAwait(false))
                {
                    if (_res.Status < 200 || _res.Status >= 300)
                    {
                        await OnTriggerDailyUpdateFailed(_req, _res).ConfigureAwait(false);
                    }


                    return;
                }
            }
        }

        internal async Task OnTriggerDailyUpdateFailed(Request req, Response res)
        {
            string content = null;
            if (res.ContentStream != null)
            {
                using (var reader = new StreamReader(res.ContentStream))
                {
                    content = await reader.ReadToEndAsync().ConfigureAwait(false);
                }
            }

            var ex = new RestApiException<ApiError>(
                req,
                res,
                content,
                Client.Deserialize<ApiError>(content)
                );
            HandleFailedTriggerDailyUpdateRequest(ex);
            HandleFailedRequest(ex);
            Client.OnFailedRequest(ex);
            throw ex;
        }

        partial void HandleFailedGetSubscriptionHistoryRequest(RestApiException ex);

        public async Task<PagedResponse<SubscriptionHistoryItem>> GetSubscriptionHistoryAsync(
            Guid id,
            int? page = default,
            int? perPage = default,
            CancellationToken cancellationToken = default
        )
        {
            if (id == default(Guid))
            {
                throw new ArgumentNullException(nameof(id));
            }

            const string apiVersion = "2019-01-16";

            var _baseUri = Client.Options.BaseUri;
            var _url = new RequestUriBuilder();
            _url.Reset(_baseUri);
            _url.AppendPath(
                "/api/subscriptions/{id}/history".Replace("{id}", Uri.EscapeDataString(Client.Serialize(id))),
                false);

            if (page != default(int?))
            {
                _url.AppendQuery("page", Client.Serialize(page));
            }
            if (perPage != default(int?))
            {
                _url.AppendQuery("perPage", Client.Serialize(perPage));
            }
            _url.AppendQuery("api-version", Client.Serialize(apiVersion));


            using (var _req = Client.Pipeline.CreateRequest())
            {
                _req.Uri = _url;
                _req.Method = RequestMethod.Get;

                using (var _res = await Client.SendAsync(_req, cancellationToken).ConfigureAwait(false))
                {
                    if (_res.Status < 200 || _res.Status >= 300)
                    {
                        await OnGetSubscriptionHistoryFailed(_req, _res).ConfigureAwait(false);
                    }

                    if (_res.ContentStream == null)
                    {
                        await OnGetSubscriptionHistoryFailed(_req, _res).ConfigureAwait(false);
                    }

                    using (var _reader = new StreamReader(_res.ContentStream))
                    {
                        var _content = await _reader.ReadToEndAsync().ConfigureAwait(false);
                        var _body = Client.Deserialize<IImmutableList<SubscriptionHistoryItem>>(_content);
                        return new PagedResponse<SubscriptionHistoryItem>(Client, OnGetSubscriptionHistoryFailed, _res, _body);
                    }
                }
            }
        }

        internal async Task OnGetSubscriptionHistoryFailed(Request req, Response res)
        {
            string content = null;
            if (res.ContentStream != null)
            {
                using (var reader = new StreamReader(res.ContentStream))
                {
                    content = await reader.ReadToEndAsync().ConfigureAwait(false);
                }
            }

            var ex = new RestApiException<ApiError>(
                req,
                res,
                content,
                Client.Deserialize<ApiError>(content)
                );
            HandleFailedGetSubscriptionHistoryRequest(ex);
            HandleFailedRequest(ex);
            Client.OnFailedRequest(ex);
            throw ex;
        }

        partial void HandleFailedRetrySubscriptionActionAsyncRequest(RestApiException ex);

        public async Task RetrySubscriptionActionAsyncAsync(
            Guid id,
            long timestamp,
            CancellationToken cancellationToken = default
        )
        {
            if (id == default(Guid))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (timestamp == default(long))
            {
                throw new ArgumentNullException(nameof(timestamp));
            }

            const string apiVersion = "2019-01-16";

            var _baseUri = Client.Options.BaseUri;
            var _url = new RequestUriBuilder();
            _url.Reset(_baseUri);
            _url.AppendPath(
                "/api/subscriptions/{id}/retry/{timestamp}".Replace("{id}", Uri.EscapeDataString(Client.Serialize(id))).Replace("{timestamp}", Uri.EscapeDataString(Client.Serialize(timestamp))),
                false);

            _url.AppendQuery("api-version", Client.Serialize(apiVersion));


            using (var _req = Client.Pipeline.CreateRequest())
            {
                _req.Uri = _url;
                _req.Method = RequestMethod.Post;

                using (var _res = await Client.SendAsync(_req, cancellationToken).ConfigureAwait(false))
                {
                    if (_res.Status < 200 || _res.Status >= 300)
                    {
                        await OnRetrySubscriptionActionAsyncFailed(_req, _res).ConfigureAwait(false);
                    }


                    return;
                }
            }
        }

        internal async Task OnRetrySubscriptionActionAsyncFailed(Request req, Response res)
        {
            string content = null;
            if (res.ContentStream != null)
            {
                using (var reader = new StreamReader(res.ContentStream))
                {
                    content = await reader.ReadToEndAsync().ConfigureAwait(false);
                }
            }

            var ex = new RestApiException<ApiError>(
                req,
                res,
                content,
                Client.Deserialize<ApiError>(content)
                );
            HandleFailedRetrySubscriptionActionAsyncRequest(ex);
            HandleFailedRequest(ex);
            Client.OnFailedRequest(ex);
            throw ex;
        }
    }
}
