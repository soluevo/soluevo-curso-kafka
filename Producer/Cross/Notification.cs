namespace Producer.Cross
{
    public class Notification<T>
    {
        public string Error { get; set; }
        public T Data { get; set; }
    }
}