// This script is placed in public domain. The author takes no responsibility for any possible harm.

var scale = 0.5f;
var speedX = 1.0f;
var speedY = 1.0f;
var calculateNormals : boolean = false;
private var baseHeight : Vector3[];

function Update () {
	var mesh : Mesh = GetComponent(MeshFilter).mesh;
	
	if (baseHeight == null)
		baseHeight = mesh.vertices;
	
	var vertices = new Vector3[baseHeight.Length];
	for (var i=0;i<vertices.Length;i++)
	{
		var vertex = baseHeight[i];
		vertex.y += Mathf.Sin(Time.time * speedY+ baseHeight[i].x + baseHeight[i].y + baseHeight[i].z) * scale;
		vertex.x += Mathf.Sin(Time.time * speedX+ baseHeight[i].x + baseHeight[i].y + baseHeight[i].z) * scale;
		vertices[i] = vertex;
	}
	mesh.vertices = vertices;
	if(calculateNormals) mesh.RecalculateNormals();
}