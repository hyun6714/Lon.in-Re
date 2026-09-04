using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArtifactDatabase", menuName = "Game/Artifact Database")]
public class ArtifactDatabase : ScriptableObject
{
    [Header("아티팩트 설정 리스트")]
    public List<ArtifactInfo> artifacts = new List<ArtifactInfo>();

    // 특정 타입의 아티팩트 정보를 쉽게 찾아오는 함수
    public ArtifactInfo GetArtifactsInfo(int id)
    {
        return artifacts.Find(c => c.artifactId == id);
    }

}
