namespace MessengerTests;

public class Tests
{
    private readonly InGroupe.Innovation.Wpf.Bedrock.Messaging.Messenger messenger = new InGroupe.Innovation.Wpf.Bedrock.Messaging.Messenger();
    public record FakeMessage(string Message);
    public record AnotherFakeMessage(string Message);
    public interface IFakeObserver { void CallMeWithMessage(object message); }

    [Fact]
    public void Send_NotifiesSubscribers_WhenSingleSubscriber()
    {
        var fakeObserver = new Mock<IFakeObserver>();
        var message = new FakeMessage(string.Empty);
        this.messenger.Subscribe<FakeMessage>(fakeObserver.Object, fakeObserver.Object.CallMeWithMessage);

        this.messenger.Send(message);

        fakeObserver.Verify(x => x.CallMeWithMessage(message), Times.Once());
    }

    [Fact]
    public void Send_DoesNotNotifySubscriber_WhenSubscriberUnsubscribes()
    {
        var fakeObserver = new Mock<IFakeObserver>();
        var message = new FakeMessage(string.Empty);
        this.messenger.Subscribe<FakeMessage>(fakeObserver.Object, fakeObserver.Object.CallMeWithMessage);

        this.messenger.Unsubscribe<FakeMessage>(fakeObserver.Object);
        this.messenger.Send(message);

        fakeObserver.Verify(x => x.CallMeWithMessage(message), Times.Never());
    }

    [Fact]
    public void Send_NotifiesAllSubscribers_WhenMoreThanOneSubscriber()
    {
        var fakeObserver1 = new Mock<IFakeObserver>();
        var fakeObserver2 = new Mock<IFakeObserver>();
        var message = new FakeMessage(string.Empty);
        this.messenger.Subscribe<FakeMessage>(fakeObserver1.Object, fakeObserver1.Object.CallMeWithMessage);
        this.messenger.Subscribe<FakeMessage>(fakeObserver2.Object, fakeObserver2.Object.CallMeWithMessage);

        this.messenger.Send(message);

        fakeObserver2.Verify(x => x.CallMeWithMessage(message), Times.Once());
        fakeObserver1.Verify(x => x.CallMeWithMessage(message), Times.Once());
    }

    [Fact]
    public void Send_NotifiesOnlySubscribersOfSpecifiedType_WhenMoreThanOneSubscriberAndTypeAreUsed()
    {
        var fakeObserver1 = new Mock<IFakeObserver>();
        var fakeObserver2 = new Mock<IFakeObserver>();
        var message = new FakeMessage(string.Empty);
        var message2 = new AnotherFakeMessage(string.Empty);
        this.messenger.Subscribe<FakeMessage>(fakeObserver1.Object, fakeObserver1.Object.CallMeWithMessage);
        this.messenger.Subscribe<AnotherFakeMessage>(fakeObserver2.Object, fakeObserver2.Object.CallMeWithMessage);

        this.messenger.Send(message);

        fakeObserver1.Verify(x => x.CallMeWithMessage(message), Times.Once());
        fakeObserver2.Verify(x => x.CallMeWithMessage(It.IsAny<object>()), Times.Never());
    }

    [Fact]
    public void Send_ThrowsException_WhenMessageIsNull()
    {
        this.messenger.Invoking(x => x.Send((FakeMessage)null))
            .Should().Throw<ArgumentNullException>().WithParameterName("message");
    }

    [Fact]
    public void Send_DoesNotThrow_WhenNoSubscribersForMessageTypeExist()
    {
        var message = new FakeMessage(string.Empty);

        this.messenger.Send(message);
    }

    [Fact]
    public void Unsubscribe_DoesNotThrow_WhenNotSubscriptionsForTheMessageTypeExist()
    {
        var fakeObserver1 = new Mock<IFakeObserver>();

        this.messenger.Unsubscribe<FakeMessage>(fakeObserver1.Object);
    }

    [Fact]
    public void Subscribe_CausesActionToBeCalled_WhenSendHappensBeforeSubscription()
    {
        var message = new FakeMessage(string.Empty);
        this.messenger.Send(message);
        var fakeObserver = new Mock<IFakeObserver>();

        this.messenger.Subscribe<FakeMessage>(fakeObserver.Object, fakeObserver.Object.CallMeWithMessage);

        fakeObserver.Verify(x => x.CallMeWithMessage(message), Times.Once());
    }
}
