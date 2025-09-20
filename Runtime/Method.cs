namespace Moths.Macros
{
    public struct Method
    {
        public void Call(params object[] args) { }
        public T Call<T>(params object[] args) => default;
    }
}