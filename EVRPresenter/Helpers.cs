﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

using MediaFoundation;
using MediaFoundation.Misc;

namespace EVRPresenter
{
    public class SamplePool : COMBase
    {
        protected Queue<IMFSample> m_VideoSampleQueue;          // Available queue

        protected bool m_bInitialized;
        protected int m_cPending;

        public SamplePool(int iSize)
        {
            m_VideoSampleQueue = new Queue<IMFSample>(iSize);
            m_bInitialized = false;
            m_cPending = 0;
        }

        //-----------------------------------------------------------------------------
        // GetSample
        //
        // Gets a sample from the pool. If no samples are available, the method
        // returns ppSample = null
        //-----------------------------------------------------------------------------

        public void GetSample(out IMFSample ppSample)
        {
            lock (this)
            {
                if (!m_bInitialized)
                {
                    throw new COMException("SamplePool::GetSample1", (int)HResult.MF_E_NOT_INITIALIZED);
                }

                if (m_VideoSampleQueue.Count == 0)
                {
                    ppSample = null;
                    return;
                }

                // Get a sample from the allocated queue.
                ppSample = (IMFSample)m_VideoSampleQueue.Dequeue();

                m_cPending++;  // Since we're locked, we don't need InterlockedIncrement
            }
        }

        //-----------------------------------------------------------------------------
        // ReturnSample
        //
        // Returns a sample to the pool.
        //-----------------------------------------------------------------------------

        public void ReturnSample(IMFSample pSample)
        {
            lock (this)
            {
                if (!m_bInitialized)
                {
                    throw new COMException("SamplePool::ReturnSample", (int)HResult.MF_E_NOT_INITIALIZED);
                }

                m_VideoSampleQueue.Enqueue(pSample);

                m_cPending--;  // Since we're locked, we don't need InterlockedDecrement
            }
        }

        //-----------------------------------------------------------------------------
        // AreSamplesPending
        //
        // Returns TRUE if any samples are in use.
        //-----------------------------------------------------------------------------

        public bool AreSamplesPending()
        {
            bool bRet;

            lock (this)
            {
                if (!m_bInitialized)
                {
                    throw new COMException("SamplePool::AreSamplesPending", (int)HResult.MF_E_NOT_INITIALIZED);
                }
                bRet = (m_cPending > 0);
            }

            return bRet;
        }


        //-----------------------------------------------------------------------------
        // Initialize
        //
        // Initializes the pool with a list of samples.
        //-----------------------------------------------------------------------------

        public void Initialize(Queue<IMFSample> samples)
        {
            lock (this)
            {
                if (m_bInitialized)
                {
                    throw new COMException("SamplePool::Initialize", (int)HResult.MF_E_INVALIDREQUEST);
                }

                Debug.Assert(m_VideoSampleQueue.Count == 0);

                m_VideoSampleQueue = samples;

                m_bInitialized = true;
            }
        }


        //-----------------------------------------------------------------------------
        // Clear
        //
        // Releases all samples.
        //-----------------------------------------------------------------------------

        public void Clear()
        {
            lock (this)
            {
                while (m_VideoSampleQueue.Count > 0)
                {
                    SafeRelease(m_VideoSampleQueue.Dequeue());
                }
                m_bInitialized = false;
                m_cPending = 0;
            }
        }
    }
}
