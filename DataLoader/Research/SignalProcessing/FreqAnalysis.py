from numpy import fft


def subfreq(s, l=0, r=1):
    ts=fft.rfft(s)
    lents=len(ts)
    ofst_l=int(lents*l)
    ofst_r=int(lents*r)
    tmp=[0]*lents
    tmp[ofst_l: ofst_r]=ts[ofst_l:ofst_r]
    return fft.irfft(tmp)


def subfreq2(s, l, r):
    ts=fft.rfft(s)
    lents=len(ts)
    tmp=[0]*ts
    tmp[l: r]=ts[l:r]
    return fft.irfft(tmp)


def convo(x, l, r):
    return [np.dot(x, np.roll(x,i)) / len(x) for i in xrange(l, r)]