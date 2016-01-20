import sklearn.linear_model as sklm


def gencases(t=10):
    ret = []
    # speed, freedays, aqi, rain, temperature, wind-speed, weather-label
    for rid in road_ids:
        X = []
        y = []
        dt = datetime(2015,7,1) + timedelta(hours=t)
        while dt < datetime(2015,12,31):
            if dt.date() in traf_restr:
                dt += timedelta(days=1)
                continue
            X.append([1 if dt.date() in freedays else 0,
                    # aqi
                    np.mean([cast_aqi[sid, dt] for sid in rmap_air[rid]]),
                    # rain
                    # np.mean([i[0] for i in [cast_weather[sid, dt] for sid
                    # in rmap_weather[rid]]]),
                    # temperature
                    np.mean([i[1] for i in [cast_weather[sid, dt] for sid in rmap_weather[rid]]]),
                    # wind-speed
                    np.mean([i[2] for i in [cast_weather[sid, dt] for sid in rmap_weather[rid]]]),
                    # weather-label
                    int(cast_weather[rmap_weather[rid][0], dt][3])])
            y.append(speed[rid, dt])
            dt+=timedelta(days=1)
        ret.append((X,y))
    return ret