using System.Collections;
 using System.Collections.Generic;
 using System.IO;
 using UnityEngine;
 
 public class LocalNetworkFileSynchronizationTest : MonoBehaviour
 {
     public string path;
 
     private void Awake()
     {
         StartCoroutine(DirSearch(path));
     }
 
     private IEnumerator DirSearch(string dir)
     {
         foreach (string f in Directory.GetFiles(dir))
         {
             yield return null;
             print(f);
         }
         foreach (string d in Directory.GetDirectories(dir))
         {
             yield return null;
             print(d);
             yield return DirSearch(d);
         }
     }
 }