using UnityEngine;
using System.Collections;
using com.dryad.effect;

public class ParticleManagerTest : MonoBehaviour {

	public string[] particleUrl;

	void Start() {
		ParticleManagerTest.Instance.Preload(particleUrl, ()=> {
			ParticleElement e = ParticleManager.GetParticle(particleUrl[0]);
			e.Play();
		});
	}
}