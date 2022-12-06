using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TestDelayer : MonoBehaviour {

    private CancellationTokenSource m_cancellationToken;

    private void Start() {
        Debug.Log("Start");
        m_cancellationToken = new CancellationTokenSource();

        Delayer.Delay(5, m_cancellationToken, () => {
            Debug.Log("delayed");
        });
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (m_cancellationToken != null) {
                Debug.Log("on click Cancel");
                m_cancellationToken.Cancel();
                m_cancellationToken.Dispose();
                m_cancellationToken = null;
            }
        }
    }

    private void OnDestroy() {
        if (m_cancellationToken != null) {
            m_cancellationToken.Cancel();
            m_cancellationToken.Dispose();
            m_cancellationToken = null;
        }
    }
}
