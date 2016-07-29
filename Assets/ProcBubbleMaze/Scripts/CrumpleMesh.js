// This script is placed in public domain. The author takes no responsibility for any possible harm.

var scale = 1.0;
var speed = 1.0;
var timeToRandomize = 10.0f;
var recalculateNormals = false;

private var baseVertices : Vector3[];
private var noise : Perlin;
private var randSeedX = 0.0f;
private var randSeedY = 0.0f;
private var randSeedZ = 0.0f;
private var countTime = 0.0f;
function Start ()
{
	noise = new Perlin();
	RandmozieSeeds();
}

function RandmozieSeeds() {
 randSeedX = ((Random.Range(0,1000)< 500?-1:1)*Random.Range(0.0f,0.1f));
 randSeedY = ((Random.Range(0,1000)< 500?-1:1)*Random.Range(0.0f,0.1f));
 randSeedZ = ((Random.Range(0,1000)< 500?-1:1)*Random.Range(0.0f,0.1f));
}

function Update () {

	countTime += Time.deltaTime;
	if(countTime >= timeToRandomize) {
		countTime = 0.0f;
		RandmozieSeeds();
	}

	var mesh : Mesh = GetComponent(MeshFilter).mesh;
	
	if (baseVertices == null)
		baseVertices = mesh.vertices;
		
	var vertices = new Vector3[baseVertices.Length];
	
	var timex = Time.time * speed + 0.1365143 + randSeedX;
	var timey = Time.time * speed + 1.21688 + randSeedY;
	var timez = Time.time * speed + 2.5564 + randSeedZ;
	for (var i=0;i<vertices.Length;i++)
	{
		var vertex = baseVertices[i];
				
		vertex.x += noise.Noise(timex + vertex.x, timex + vertex.y, timex + vertex.z) * scale;
		vertex.y += noise.Noise(timey + vertex.x, timey + vertex.y, timey + vertex.z) * scale;
		vertex.z += noise.Noise(timez + vertex.x, timez + vertex.y, timez + vertex.z) * scale;
		
		vertices[i] = vertex;
	}
	
	mesh.vertices = vertices;
	
	if (recalculateNormals)	
		mesh.RecalculateNormals();
	mesh.RecalculateBounds();
}