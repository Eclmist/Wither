using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Any object/character that can be damaged should implement this interface
/// </summary>
public interface IDamagable {

    void TakeDamage(float damage);
    void ApplyStun(float duration);

    IEnumerator StatusEffect(float duration);

	
}
