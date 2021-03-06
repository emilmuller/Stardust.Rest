﻿using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stardust.Aadb2c.AuthenticationFilter.Core;
using Stardust.Aadb2c.AuthenticationFilter;

namespace Stardust.Aadb2c.AuthenticationFilter
{
    public class B2COAuthHandler : RemoteAuthenticationHandler<B2COptions>
    {
        public B2COAuthHandler(IOptionsMonitor<B2COptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            try
            {
                Logger.LogDebug($"Looking for bearer token! {nameof(HandleRemoteAuthenticateAsync)}");
                var v = Request.Headers["Authorization"].FirstOrDefault();
                var auth = v?.Split(' ') ?? new string[0];
                if (v != null && auth.Length >= 2 && v.Split(' ')[0].Equals("bearer", StringComparison.InvariantCultureIgnoreCase))
                {
                    var user = TokenValidator.Validate(v.Split(' ')[1]);
                    Context.User = user;

                    return Task.FromResult(HandleRequestResult.Success(new AuthenticationTicket(user, Scheme.Name)));
                }
                Logger.LogDebug("No bearer token provided");
                return Task.FromResult(HandleRequestResult.SkipHandler());
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return Task.FromResult(HandleRequestResult.Fail(ex));
            }
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                Logger.LogDebug($"Looking for bearer token! {nameof(HandleAuthenticateAsync)}");
                var v = Request.Headers["Authorization"].FirstOrDefault();
                if (v != null && v.Split(' ')[0].Equals("bearer", StringComparison.InvariantCultureIgnoreCase))
                {
                    var user = TokenValidator.Validate(v.Split(' ')[1]);
                    Context.User = user;

                    return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(user, Scheme.Name)));
                }
                Logger.LogDebug("No bearer token provided");
                return Task.FromResult(AuthenticateResult.NoResult());
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return Task.FromResult(AuthenticateResult.Fail(ex));
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Logger.LogDebug("Test");

            return base.HandleChallengeAsync(properties);
        }
        //protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        //{

        //}
    }
}
