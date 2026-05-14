using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeApp
{
    interface IObserver
    {
        void Notify(int value);
    }
    class Hanumant : IObserver
    {
        private string _name;
        public Hanumant(string name)
        {
            _name = name;
        }
        public void Notify(int value)
        {
            Console.WriteLine($"{_name} received notification: {value}");
        }
    }

    class  Pratiksha : IObserver
    {
        private string _name;
        public Pratiksha(string name)
        {
            _name = name;
        }
        public void Notify(int value)
        {
            Console.WriteLine($"{_name} received notification: {value}");
        }
    }
    interface IObserveble
    {
        void Subscribe(IObserver observer);
        void Unsubscribe(IObserver observer);
        void Notify(string videoTitle);
    }
   class YoutubeChannel : IObserveble
    {
        private List<IObserver> _observers = new List<IObserver>();
        public void Subscribe(IObserver observer)
        {
            _observers.Add(observer);
        }
        public void Unsubscribe(IObserver observer)
        {
            _observers.Remove(observer);
        }
        public void Notify(string videoTitle)
        {
            foreach (var observer in _observers)
            {
                observer.Notify(videoTitle.GetHashCode());
            }
        }
    }

    internal class Youtube_ObserverPattern
    {
        public Youtube_ObserverPattern()
        {
            YoutubeChannel channel = new YoutubeChannel();
            Hanumant hanumant = new Hanumant("Hanumant");
            Pratiksha pratiksha = new Pratiksha("Pratiksha");
            channel.Subscribe(hanumant);
            channel.Subscribe(pratiksha);
            channel.Notify("New Video Uploaded: Observer Pattern in C#");
        }
    }

}
