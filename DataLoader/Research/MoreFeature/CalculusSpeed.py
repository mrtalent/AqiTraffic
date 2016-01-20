def calculus_speed():
    global road_ids, speed, cast_aqi, rmap_air
    ret = {}
    ret_aqi = {}
    for rid in road_ids:
        day = datetime(2015, 7, 1)
        while day < datetime(2015, 12, 31):
            # from 7:30 to 21:45
            ret[rid, day] = np.mean([speed[rid, day + timedelta(minutes=i * 15)] for i in xrange(7 * 4 + 2, 22 * 4)])
            ret_aqi[rid, day] = np.mean([np.mean([cast_aqi[sid, day + timedelta(hours=7.5, minutes=i * 15)] for sid in rmap_air[rid]]) for i in
                     xrange(8)])
            day+=timedelta(days=1)
    return ret, ret_aqi


speedcal, aqical = calculus_speed()