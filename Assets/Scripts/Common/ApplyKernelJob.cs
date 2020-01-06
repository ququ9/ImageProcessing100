using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct ApplyKernelJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<Color32> Pixels;

    [ReadOnly, DeallocateOnJobCompletion] public NativeArray<float> Kernel;

    [WriteOnly] public NativeArray<Color32> Result;

    [ReadOnly] private int KernelWidth;
    [ReadOnly] private int KernelCenterX;
    [ReadOnly] private int KernelCenterY;
    [ReadOnly] public int Width;
    [ReadOnly] public int Height;

    public static ApplyKernelJob Create(TextureData source, ConvolutionKernel kernel)
    {
        var job = new ApplyKernelJob {
            Pixels = source.Pixels, 
            Width = source.Width, 
            Height = source.Height, 
            Kernel = new NativeArray<float>(kernel.Weights.ToArray(), Allocator.TempJob), 
            KernelWidth = kernel.KernelWidth,
            KernelCenterX = kernel.KernelCenterX,
            KernelCenterY = kernel.KernelCenterY,
            Result = new NativeArray<Color32>((source.Width * source.Height), Allocator.TempJob, NativeArrayOptions.UninitializedMemory)
        };
        return job;
    }

    public void Execute(int index)
    {
        var pixelY = (index / this.Width);
        var pixelX = (index - (pixelY * this.Width));

        float r = 0.0f, g = 0.0f, b = 0.0f;
        for (var iKernel = 0; iKernel < Kernel.Length; ++iKernel) {
            var weight = this.Kernel[iKernel];

            var yKernel = (iKernel / this.KernelWidth);
            var xKernel = (iKernel - (yKernel * this.KernelWidth));

            var x = Mathf.Clamp(pixelX + xKernel - this.KernelCenterX, 0, this.Width - 1);
            var y = Mathf.Clamp(pixelY + yKernel - this.KernelCenterY, 0, this.Height - 1);
            var c = this.Pixels[(y * this.Width) + x];
            r += (weight * c.r);
            g += (weight * c.g);
            b += (weight * c.b);
        }

        var o = this.Pixels[index];
        this.Result[index] = new Color32((byte)Mathf.Clamp((int)r, 0, 255) , (byte)Mathf.Clamp((int)g, 0, 255), (byte)Mathf.Clamp((int)b, 0, 255), o.a);
    }
}