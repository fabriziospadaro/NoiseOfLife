using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class CellularAutomaton {
  private byte[,] cells;
  private float[,] heat_map;
  private int sizeX;
  private int sizeY;
  private int precision;
  private int smothness;
  private Dictionary<string, int> lineFix;
  public CellularAutomaton(int sizeX, int sizeY, float fill_percentage, int smothness = 2, int precision = 1, int seed = 0) {
    //randomizing unity seed
    UnityEngine.Random.InitState(seed == 0 ? UnityEngine.Random.Range(-99999, 99999) : seed);
    this.precision = precision;
    this.smothness = smothness;
    lineFix = new Dictionary<string, int>();
    this.sizeX = sizeX;
    this.sizeY = sizeY;
    cells = new byte[sizeX, sizeY];
    heat_map = new float[sizeX, sizeY];
    //init map
    for(int x = 0; x < sizeX; x++) {
      for(int y = 0; y < sizeY; y++) {
        cells[x, y] = UnityEngine.Random.Range(0, 100) < fill_percentage ? (byte)1 : (byte)0;
      }
    }
  }
  public float[,] LifeNoise(int iteretions = 10000) {
    int total_pixels = sizeX * sizeY;
    for(int i = 0; i < iteretions; i++) {
      int changed = Step();
      if(changed / (float)total_pixels <= 0.0001f)
        break;
    }
    Blur blur = new Blur(smothness, precision);
    heat_map = blur.FastBlur(heat_map);
    AdjustValues();
    return heat_map;
  }

  public void AdjustValues() {
    float min = float.MaxValue;
    float max = float.MinValue;
    for(int x = 0; x < sizeX; x++) {
      for(int y = 0; y < sizeY; y++) {
        if(heat_map[x, y] < min)
          min = heat_map[x, y];
        else if(heat_map[x, y] > max)
          max = heat_map[x, y];
      }
    }
    max -= min;
    for(int x = 0; x < sizeX; x++) {
      for(int y = 0; y < sizeY; y++) {
        heat_map[x, y] = (heat_map[x, y] - min) / max;
      }
    }
  }

  private int GetNeighbor(int x, int y) {
    int neighbor = 0;
    for(int j = -1; j < 2; j++)
      for(int k = -1; k < 2; k++)
        if(!(j == 0 && k == 0)) {
          int _x = (x + j + sizeX) % sizeX;
          int _y = (y + k + sizeY) % sizeY;
          neighbor += cells[_x, _y];
        }

    return neighbor;
  }

  private int Step() {
    int pixels_changed = 0;
    float pixels = Mathf.Sqrt(sizeX * (float)sizeY);
    byte[,] temp_cells = new byte[sizeX, sizeY];
    for(int x = 0; x < sizeX; x++)
      for(int y = 0; y < sizeY; y++) {
        int n = GetNeighbor(x, y);
        float heat = n / 8f;
        temp_cells[x, y] = GetValueFromRules(x, y, n);
        pixels_changed += cells[x, y] != temp_cells[x, y] ? 1 : 0;
        if(temp_cells[x, y] != cells[x, y]) {
          heat_map[x, y] += heat * (pixels / 2);
        }
        else if(temp_cells[x, y] == 0) {
          heat_map[x, y] -= heat / pixels;
        }
      }
    cells = temp_cells;
    RemoveLinePattern();
    return pixels_changed;
  }

  //there are way better ways to recognize a pattern
  private void RemoveLinePattern() {
    for(int x = 0; x < sizeX; x++) {
      for(int y = 0; y < sizeY; y++) {
        //check bounds
        if(x < sizeX - 2 && cells[x, y] == 1 && cells[x + 1, y] == 1 && cells[x + 2, y] == 1) {
          if(lineFix.ContainsKey("x+" + x + "y" + y))
            lineFix["x+" + x + "y" + y] += 1;
          else
            lineFix["x+" + x + "y" + y] = 0;

          if(lineFix["x+" + x + "y" + y] > 4) {
            cells[x, y] = cells[x + 1, y] = cells[x + 2, y] = 0;
            lineFix["x+" + x + "y" + y] = 0;
          }
        }
        //check bounds
        else if(y < sizeY - 2 && cells[x, y] == 1 && cells[x, y + 1] == 1 && cells[x, y + 2] == 1) {
          if(lineFix.ContainsKey("x" + x + "y+" + y))
            lineFix["x" + x + "y+" + y] += 1;
          else
            lineFix["x" + x + "y+" + y] = 0;

          if(lineFix["x" + x + "y+" + y] > 4) {
            cells[x, y] = cells[x, y + 1] = cells[x, y + 2] = 0;
            lineFix["x" + x + "y+" + y] = 0;
          }
        }
      }
    }
  }

  private byte GetValueFromRules(int x, int y, int n) {
    byte status = cells[x, y];
    if(status == 0 && n == 3)
      return 1;
    else if(status == 1 && (n < 2 || n > 3))
      return 0;
    else
      return status;
  }

}