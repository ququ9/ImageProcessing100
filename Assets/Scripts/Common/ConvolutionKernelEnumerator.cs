using System;
using System.Collections;
using System.Collections.Generic;

public class ConvolutionKernelEnumerator : IEnumerator<(int X, int Y, int OffsetX, int OffsetY)>
{
    private int _currentIndex;

    private readonly int _kernelSize;
    private readonly int _kernelCenter;
    private readonly int _pixelCount;

    public ConvolutionKernelEnumerator(int kernelSize)
    {
        _kernelSize = kernelSize;
        _kernelCenter = (_kernelSize / 2);
        _pixelCount = (kernelSize * kernelSize);

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

    public (int X, int Y, int OffsetX, int OffsetY) Current
    {
        get {
            var y = (_currentIndex / _kernelSize);
            // var x = (_currentIndex % _width);
            var x = (_currentIndex - (y * _kernelSize));

            // 処理するピクセルに対して中心からの距離を返す([-kernel_size/2, kernel_size/2], [-kernel_size/2, kernel_size/2])
            return (x, y, x - _kernelCenter, y - _kernelCenter);
        }
    }

    object IEnumerator.Current => this.Current;
}