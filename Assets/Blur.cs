using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blur{
  private float avgValue = 0;
  private float blurPixelCount = 0;
  public int radius = 2;
  public int iterations = 2;
  public Blur(int radius,int iterations) {
    this.radius = radius;
    this.iterations = iterations;
  }
  public float[,] FastBlur(float[,] values)
  {
    float[,] out_values = values;
    for(var i = 0; i < iterations; i++)
    {
      out_values = BlurArray(out_values,true);
      out_values = BlurArray(out_values,false);
    }
    return out_values;
  }

  private float[,] BlurArray(float[,] values,bool horizontal)
  {
    int width = values.GetLength(0);
    int height = values.GetLength(1);
    float[,] blurred = new float[width,height];
    int _W = width;
    int _H = height;
    int xx, yy, x, y;

    if(horizontal)
    {
      for(yy = 0; yy < _H; yy++)
      {
        for(xx = 0; xx < _W; xx++)
        {
          ResetValue();

          for(x = xx; (x < xx + radius && x < _W); x++)
          {
            AddValue(values[x,yy]);
          }

          for(x = xx; (x > xx - radius && x > 0); x--)
          {
            AddValue(values[x,yy]);

          }


          CalcValue();

          for(x = xx; x < xx + radius && x < _W; x++)
          {
            blurred[x,yy] = avgValue;
          }
        }
      }
    }

    else
    {
      for(xx = 0; xx < _W; xx++)
      {
        for(yy = 0; yy < _H; yy++)
        {
          ResetValue();

          for(y = yy; (y < yy + radius && y < _H); y++)
          {
            AddValue(values[xx,y]);
          }

          for(y = yy; (y > yy - radius && y > 0); y--)
          {
            AddValue(values[xx,y]);
          }
          CalcValue();
          for(y = yy; y < yy + radius && y < _H; y++)
          {
            blurred[xx,y] = avgValue;
          }
        }
      }
    }
    return blurred;
  }
  private void AddValue(float value)
  {
    avgValue += value;
    blurPixelCount++;
  }

  private void ResetValue()
  {
    avgValue = 0.0f;
    blurPixelCount = 0;
  }

  private void CalcValue()
  {
    avgValue = avgValue / blurPixelCount;
  }
}
