using System;
using System.Collections;
using System.Collections.Generic;

public class TextureDataEnumerator : IEnumerator<(int X, int Y)>
{
    private int _currentIndex;

    private readonly int _width;
    private readonly int _height;
    private readonly int _pixelCount;

    public TextureDataEnumerator(int width, int height)
    {
        _width = width;
        _height = height;
        _pixelCount = (width * height);

        _currentIndex = 0;
    }

    public bool MoveNext()
    {
        if ((_currentIndex + 1) >= _pixelCount) {
            return false;
        }

        ++_currentIndex;
        return true;
    }

    public void Reset()
    {
        _currentIndex = 0;
    }

    void IDisposable.Dispose() { }

    public (int X, int Y) Current
    {
        get {
            var y = (_currentIndex / _width);
            // var x = (_currentIndex % _width);
            var x = (_currentIndex - (y * _width));
            return (x, y);
        }
    }

    object IEnumerator.Current => this.Current;
}