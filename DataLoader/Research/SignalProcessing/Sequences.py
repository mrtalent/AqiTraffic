def sele(x,y,wk):
    if wk == 0:
        return [i for i in x if i.date() not in freedays], [
                y[i] for i in xrange(len(y)) if i.date() not in freedays]
    elif wk == 1:
        return [i for i in x if i.date() in freedays], [
                y[i] for i in xrange(len(y)) if i.date() in freedays]
    else:
        return x,y


def aqi_seq(rid, dbound=(dt_oct, edt), tdelta=timedelta(minutes=15), ofst=timedelta(minutes=0), wk=2):
    dt = dbound[0] + ofst
    x = []
    y = []
    while dt < dbound[1]:
        aqilist = [cast_aqi[sid, dt] for sid in rmap_air[rid] if cast_aqi[sid, dt] != -1]
        if aqilist == []:
            dt+=tdelta
            continue
        x.append(dt)
        y.append(np.mean(aqilist))
        dt+=tdelta
    return sele(x,y,wk)


def speed_seq(rid, dbound=(dt_oct, edt), tdelta=timedelta(minutes=15), ofst=timedelta(minutes=0), wk=2):
    dt = dbound[0] + ofst
    x = []
    y = []
    while dt < dbound[1]:
        x.append(dt)
        y.append(speed[rid, dt])
        dt+=tdelta
    return sele(x,y,wk)