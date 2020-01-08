using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
  private CellularAutomaton automaton;
  public Texture2D texture;
  public Gradient gradient;
  // Start is called before the first frame update
  void Start() {
    texture = new Texture2D(256, 256);
    automaton = new CellularAutomaton(256, 256, 35, 4, 1);
    float[,] values = automaton.LifeNoise();
    texture.SetPixels32(floatToColor32(values));
    texture.Apply();
  }

  Color32[] floatToColor32(float[,] f) {
    Color32[] colors = new Color32[f.Length];
    int id = 0;
    for(int x = 0; x < f.GetLength(0); x++) {
      for(int y = 0; y < f.GetLength(1); y++) {
        colors[id] = gradient.Evaluate(f[x, y]);
        id++;
      }
    }
    return colors;
  }
}

