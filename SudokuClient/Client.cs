using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Grpc.Core;
using Grpc.Net.Client;

using ReactiveUI;

using Sudoku;

using SudokuClient.Models;

namespace SudokuClient
{
    public class Client
    {
        public Player Player { get; private set; } = new();
        public Field? Field { get; private set; }

        public IObservable<Unit> SuccessLoginEvent => _loginSubject;
        public IObservable<Unit> PLayStartEvent => _playSubject;
        public IObservable<bool> TurnEvent => _turnSubject;
        public IObservable<string> DisconnectedEvent => _disconnectedSubject;

        private readonly GrpcChannel _channel;
        private readonly AsyncDuplexStreamingCall<Request, Event> _stream;
        private readonly Subject<Unit> _loginSubject = new();
        private readonly Subject<Unit> _playSubject = new();
        private readonly Subject<bool> _turnSubject = new();
        private readonly Subject<string> _disconnectedSubject = new();
        private bool _alreadyDisconnected = false;

        public Client(string address)
        {
            _channel = GrpcChannel.ForAddress(address);
            var client = new SudokuService.SudokuServiceClient(_channel);
            _stream = client.Connect();
            Task.Run(ReadEvents);
        }

        public async Task LoginRequest(string login)
        {
            Player.Login = login;
            try
            {
                var loginRequest = new LoginRequest { Login = login };
                var request = new Request { Login = loginRequest };
                await _stream.RequestStream.WriteAsync(request);
            }
            catch
            {
                OnDisconnected("Server is unreachable.");
            }
        }

        public async Task PlayRequest()
        {
            try
            {
                var playRequest = new PlayRequest { };
                var request = new Request { Play = playRequest };
                await _stream.RequestStream.WriteAsync(request);
            }
            catch
            {
                OnDisconnected("Server is unreachable.");
            }
        }

        public async Task TurnRequest(Point point)
        {
            try
            {
                var turnRequest = new TurnRequset { Point = point };
                var request = new Request { Turn = turnRequest };
                await _stream.RequestStream.WriteAsync(request);
            }
            catch
            {
                OnDisconnected("Server is unreachable.");
            }
        }

        private async Task ReadEvents()
        {
            try
            {
                var stream = _stream.ResponseStream;
                while (await stream.MoveNext(CancellationToken.None))
                {
                    switch (stream.Current.EventCase)
                    {
                        case Event.EventOneofCase.None:
                            throw new InvalidOperationException();
                        case Event.EventOneofCase.Login:
                            if (!stream.Current.Login.Success)
                            {
                                OnDisconnected("User with this login is already on the server.");
                                break;
                            }
                            Player.Score = stream.Current.Login.Score;
                            _loginSubject.OnNext(Unit.Default);
                            break;
                        case Event.EventOneofCase.Play:
                            Field = new Field(stream.Current.Play.Points);
                            _playSubject.OnNext(Unit.Default);
                            break;
                        case Event.EventOneofCase.Error:
                            OnDisconnected(stream.Current.Error.Error);
                            break;
                        case Event.EventOneofCase.Turn:
                            _turnSubject.OnNext(stream.Current.Turn.Success);
                            break;
                        case Event.EventOneofCase.Win:
                            throw new InvalidOperationException();
                            break;
                    }
                }
            }
            catch
            {
                OnDisconnected("Server is unreachable.");
            }
        }

        private void OnDisconnected(string message)
        {
            RxApp.MainThreadScheduler.Schedule(Unit.Default, (scheduler, state) =>
            {
              
                if (_alreadyDisconnected)
                    return Disposable.Empty;
                _alreadyDisconnected = true;
                _disconnectedSubject.OnNext(message);                    

                return Disposable.Empty;
                
            });
        }

    }
}
