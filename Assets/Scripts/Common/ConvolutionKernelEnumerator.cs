using System;
using System.Collections;
using System.Collections.Generic;

public class ConvolutionKernelEnumerator : IEnumerator<(int X, int Y, int OffsetX, int OffsetY)>
{
    private int _currentIndex;

    private readonly int _kernelWidth;
    private readonly int _kernelHeight;
    private readonly int _kernelCenterX;
    private readonly int _kernelCenterY;
    private readonly int _pixelCount;

    public ConvolutionKernelEnumerator(int kernelWidth, int kernelHeight)
    {
        _kernelWidth = kernelWidth;
        _kernelHeight = kernelHeight;

        _kernelCenterX = _kernelWidth / 2;
        _kernelCenterY = kernelHeight / 2;
        _pixelCount = (kernelWidth * kernelHeight);

        _currentIndex = -1;
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
        _currentIndex = -1;
    }

    void IDisposable.Dispose() { }

    public (int X, int Y, int OffsetX, int OffsetY) Current
    {
        get {
            var y = (_currentIndex / _kernelWidth);
            // var x = (_currentIndex % _width);
            var x = (_currentIndex - (y * _kernelWidth));

            // 処理するピクセルに対して中心からの距離を返す([-kernel_size/2, kernel_size/2], [-kernel_size/2, kernel_size/2])
            return (x, y, x - _kernelCenterX, y - _kernelCenterY);
        }
    }

    object IEnumerator.Current => this.Current;
}