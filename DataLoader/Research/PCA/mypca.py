def speed_segs(rid, wk=2):
    y = speed_seq(rid, dbound=(sdt, edt), wk=wk)[1]
    return np.array(y).reshape(-1,96)


def PCDim(X, ratio=0.9):
    X-=np.mean(X, axis=0)
    cov = X.T.dot(X)
    L,V = np.linalg.eig(cov)
    tmpsum = sum(LD)
    for i in xrange(len(L)):
        if sum(L[:i + 1]) >= tmpsum * ratio:
            break
    return i