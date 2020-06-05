﻿using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace IqfeedKeepAlive
{
    /// <summary>
    /// Extends the Socket class.
    /// </summary>
    internal static class SocketExtensions
    {
        /// <summary>
        /// Establishes a connection to a remote host. The host is specified by a host name
        /// and a port number. Allows the operation to be cancelled via a
        /// CancellationToken.
        /// </summary>
        /// <param name="socket">The socket to perform the connect operation on.</param>
        /// <param name="host">The name of the remote host.</param>
        /// <param name="port">The port number of the remote host.</param>
        /// <param name="token">The token to check for cancellation.</param>
        /// <exception cref="OperationCanceledException">If the task is cancelled before
        /// completion.</exception>
        public static async Task ConnectAsync(
            this Socket socket, string host, int port, CancellationToken token)
        {
            if (socket == null)
                throw new ArgumentNullException(nameof(socket));

            token.ThrowIfCancellationRequested();
            await using var register = token.Register(socket.Close);
            token.ThrowIfCancellationRequested();

            try
            {
                await socket.ConnectAsync(host, port);
            }
            catch (NullReferenceException) when (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }
        }
    }
}
