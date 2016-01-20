import matplotlib.pyplot as plt
import matplotlib
from matplotlib.font_manager import FontProperties
import numpy as np
from datetime import *
import random
from spyderlib.utils.iofuncs import load_dictionary
import sys


sdt = datetime(2015,7,1)
dt_oct = datetime(2015,10,1)
edt = datetime(2015,12,31)
traf_restr = [date(2015,8,20) + timedelta(days=i) for i in xrange(15)]
traf_restr.extend([date(2015,12,8) + timedelta(days=i) for i in xrange(3)])
traf_restr.extend([date(2015,12,19) + timedelta(days=i) for i in xrange(4)])
weather_labels = ['Sunny','Cloudy','Overcast','Rainy','Sprinkle','ModerateRain',
        'HeavyRain','Rainstorm','Thunderstorm','FreezingRain','Snowy','LightSnow',
        'ModerateSnow','HeavySnow','Foggy','Sandstorm','Dusty']


def gen_freedays():
    txt = open(r'D:\v-tianhe\aqiTraffic\workspace\Data\TrafficRestriction.csv').read().split('\n')
    ret = []
    for i,iline in enumerate(txt):
        if iline.split(',')[-1] != '0':
            ret.append(date(2015,7,1) + timedelta(days=i))
    return ret
freedays = gen_freedays()


def add_axes_attr(ptitle, pxlabel, pylabel, pts=-1, pls=-1):
    def _add_axes_attr(inbox_module):
        def outofbox(*args, **kwargs):
            if kwargs.has_key('ax'):
                ax = kwargs['ax']
            else:
                ax = plt.gca()
            if pts == -1:
                ax.set_title(ptitle)
            else:
                ax.set_title(ptitle, fontsize=pts)
            if pls == -1:
                ax.set_xlabel(pxlabel)
                ax.set_ylabel(pylabel)
            else:
                ax.set_xlabel(pxlabel, fontsize=pls)
                ax.set_ylabel(pylabel, fontsize=pls)
            if kwargs.has_key('ax'):
                return inbox_module(*args, **kwargs)
            else:
                return inbox_module(ax=plt.gca(), *args, **kwargs)
        return outofbox
    return _add_axes_attr


def get_as_cases(rid, tdelta=timedelta(minutes=30), tbound=(time(10,0,0), time(10,30,0)), dbound=(dt_oct, edt), wk=2):
    global speed, cast_aqi, rmap_air
    x_workday = []
    y_workday = []
    x_weekend = []
    y_weekend = []
    lowt = tbound[0]
    upt = tbound[1]
    dt = dbound[0]
    while dt < dbound[1]:
        if (not lowt <= dt.time() < upt) or (dt.date() in traf_restr):
            dt+=tdelta
            continue
        aqi_list = [cast_aqi[i, dt] for i in rmap_air[rid] if cast_aqi[i, dt] != -1]
        # overlook those with no aqi record
        if aqi_list == []:
            dt+=tdelta
            continue
        # AQI = mean value of all stations recorded aqi
        if dt.date() not in freedays:
            x_workday.append(np.mean(aqi_list))
            y_workday.append(speed[rid, dt])
        else:
            x_weekend.append(np.mean(aqi_list))
            y_weekend.append(speed[rid, dt])
        dt+=tdelta
    if wk == 0:
        return x_workday, y_workday
    if wk == 1:
        return x_weekend, y_weekend
    if wk == 2:
        x_workday.extend(x_weekend)
        y_workday.extend(y_weekend)
        return x_workday, y_workday


def get_asi_cases(rid, tdelta=timedelta(minutes=30), tbound=(time(10,0,0), time(10,30,0)), dbound=(dt_oct, edt), wk=2):
    global speed_index, cast_aqi, rmap_air
    x_workday = []
    y_workday = []
    x_weekend = []
    y_weekend = []
    lowt = tbound[0]
    upt = tbound[1]
    dt = dbound[0]
    while dt < dbound[1]:
        if (not lowt <= dt.time() < upt) or (dt.date() in traf_restr):
            dt+=tdelta
            continue
        aqi_list = [cast_aqi[i, dt] for i in rmap_air[rid] if cast_aqi[i, dt] != -1]
        # overlook those with no aqi record
        if aqi_list == []:
            dt+=tdelta
            continue
        # AQI = mean value of all stations recorded aqi
        if dt.date() not in freedays:
            x_workday.append(np.mean(aqi_list))
            y_workday.append(speed_index[rid, dt])
        else:
            x_weekend.append(np.mean(aqi_list))
            y_weekend.append(speed_index[rid, dt])
        dt+=tdelta
    if wk == 0:
        return x_workday, y_workday
    if wk == 1:
        return x_weekend, y_weekend
    if wk == 2:
        x_workday.extend(x_weekend)
        y_workday.extend(y_weekend)
        return x_workday, y_workday


@add_axes_attr('Record Cases of AQI and Speed', 'AQI', 'Speed (m/s)')
def asplot(rid, t=10, ax=plt.gca(), dbound=(dt_oct, edt), wk=2):
    ax.set_ylim(0,130)
    ax.set_xlim(0,700)
    x, y = get_as_cases(rid, tbound=(time(t,0,0), time(t,30,0)), dbound=dbound, wk=wk)
    if wk == 0:
        ax.plot(x, y, '.', label='Workday')
        return
    if wk == 1:
        ax.plot(x, y, '.', label='Weekend')
        return
    ax.plot(x,y,'.')


# Return list of tuple
# Sorted by linear correlation coefficients
# In decent order
def sort_in_linear_correlation(t=10, dbound=(dt_oct, edt), wk=2):
    global road_ids
    road_lcorr = {}
    for rid in road_ids:
        x, y = get_as_cases(rid, tbound=(time(t,0,0), time(t,30,0)), dbound=dbound, wk=wk)
        road_lcorr[rid] = np.corrcoef(x,y)[0][1]
    return sorted(road_lcorr.iteritems(), key = lambda (k,v): -abs(v))


def sort_in_minmaxdiff(t=10, dbound=(dt_oct, edt), wk=2):
    global road_ids
    road_diff = {}
    for rid in road_ids:
        x, y = get_as_cases(rid, tbound=(time(t,0,0), time(t,30,0)), dbound=dbound, wk=wk)
        road_diff[rid] = max(y) - min(y)
    return sorted(road_diff.iteritems(), key = lambda (k,v): v)


def sort_in_variance(t=10, dbound=(dt_oct, edt), wk=2):
    global road_ids
    road_vars = {}
    for rid in road_ids:
        x,y = get_as_cases(rid, tbound=(time(t,0,0), time(t,30,0)), dbound=dbound, wk=wk)
        road_vars[rid] = np.var(y)
    return sorted(road_vars.iteritems(), key = lambda (k,v): -abs(v))


""" Analysis from figures """
def speed_change(rid, ax=plt.gca(), dbound=(sdt, edt), tdelta=timedelta(minutes=15), ofst=timedelta(0)):
    global cast_aqi, rmap_air, speed
    x = []
    y = []
    dt = dbound[0] + ofst
    while dt < dbound[1]:
        x.append(dt)
        y.append(speed[rid, dt])
        dt+=tdelta
    ax.plot(x, y, '-r', label='Speed Change')


def aqi_change(rid, ax=plt.gca(), dbound=(sdt, edt), tdelta=timedelta(minutes=15), ofst=timedelta(0)):
    global cast_aqi, rmap_air
    x = []
    y = []
    dt = dbound[0] + ofst
    while dt < dbound[1]:
        aqi_list = [cast_aqi[i, dt] for i in rmap_air[rid] if cast_aqi[i, dt] != -1]
        if aqi_list == []:
            dt+=tdelta
            continue
        x.append(dt)
        y.append(np.mean(aqi_list))
        dt+=tdelta
    ax.plot(x,y,'-b', label='AQI Change')


alldata = load_dictionary(r'D:\v-tianhe\aqiTraffic\workspace\Data\all.spydata')[0]
for k in alldata.keys():
    exec("%s=alldata['%s']" % (k,k))
del(alldata)