using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct ApplyKernelJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<Color32> Pixels;

    [ReadOnly, DeallocateOnJobCompletion] public NativeArray<float> Kernel;

    [WriteOnly] public NativeArray<Color32> Result;

    [ReadOnly] public int KernelSize;
    [ReadOnly] public int Width;
    [ReadOnly] public int Height;

    public void Execute(int index)
    {
        var pixelY = (index / this.Width);
        var pixelX = (index - (pixelY * this.Width));
        var kernelCenter = this.KernelSize / 2;

        float r = 0.0f, g = 0.0f, b = 0.0f;
        for (var iKernel = 0; iKernel < Kernel.Length; ++iKernel) {
            var weight = this.Kernel[iKernel];

            var yKernel = (iKernel / KernelSize);
            var xKernel = (iKernel - (yKernel * KernelSize));

            var x = Mathf.Clamp(pixelX + xKernel - kernelCenter, 0, this.Width - 1);
            var y = Mathf.Clamp(pixelY + yKernel - kernelCenter, 0, this.Height - 1);
            var c = this.Pixels[(y * this.Width) + x];
            r += (weight * c.r);
            g += (weight * c.g);
            b += (weight * c.b);
        }

        var pixelIndex = ((pixelY * this.Width) + pixelX);
        var o = this.Pixels[pixelIndex];
        this.Result[pixelIndex] = new Color32((byte)Mathf.Clamp((int)r, 0, 255) , (byte)Mathf.Clamp((int)g, 0, 255), (byte)Mathf.Clamp((int)b, 0, 255), o.a);
    }
}