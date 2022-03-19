#include "sofa.h"
void reprd ( char*, double, double );
int main ()
{
    iauASTROM astrom;
    iauLDBODY b[3];
    double phi, elong, hm, phpa, tc, rh, wl, utc1, utc2, tai1, tai2,
            tt1, tt2, xp, yp, dut1, dx, dy, rc, dc, pr, pd, px, rv,
            eo, ri, di, rca, dca, ra, da, aot, zot, hot, dot, rot,
            aob, zob, hob, dob, rob, testrc, testdc,
            pvh[2][3], pvb[2][3], r[3][3], x, y, s;
/* Site longitude, latitude (radians) and height above the geoid (m). */
    iauAf2a ( ' ',34 , 45, 52.9194, &elong );
    iauAf2a ( ' ', 32, 4, 4.0794, &phi );
    hm = 5;
/* Ambient pressure (HPa), temperature (C) and rel. humidity (frac). */
    phpa = 1013.0;
    tc = 16;
    rh = 0.50;
/* Effective color (microns). */
    wl = 0.55;
/* UTC date. */
    if ( iauDtf2d ( "UTC", 2021, 4, 11, 20, 00, 00,
                    &utc1, &utc2 ) ) return -1;
/* TT date. */
    if ( iauUtctai ( utc1, utc2, &tai1, &tai2 ) ) return -1;
    if ( iauTaitt ( tai1, tai2, &tt1, &tt2 ) ) return -1;
/* EOPs: polar motion in radians, UT1-UTC in seconds. */
    xp = 0.0894 * DAS2R;
    yp = 0.4203 * DAS2R;
    dut1 = -0.17617;
/* Corrections to IAU 2000A CIP (radians). */
    dx = 0.269e-3 * DAS2R;
    dy = -0.274e-3 * DAS2R;
/* Star ICRS RA,Dec (radians). */
    if ( iauTf2a ( ' ', 14, 34, 16.81183, &rc ) ) return -1;
    if ( iauAf2a ( '-', 12, 31, 10.3965, &dc ) ) return -1;
    testrc = (60.0*( 60.0 *(0.152887)))*DS2R;
    testdc =(60.0*(60.0 *59.14978))*DAS2R;
    reprd ( "ICRS, epoch J2000.0:", rc, dc );
/* Annual proper motion: RA/Dec derivatives, epoch J2000.0. */
    pr = atan2 ( 523.39e-3 * DAS2R, cos(dc) );
    pd = -180.42e-3 * DAS2R;
/* Parallax (arcsec) and recession speed (km/s). */
    px = 0.0595798;
    rv = 0.0;


/* ICRS to observed. */
    if ( iauAtco13 ( testrc, testdc, pr, pd, px, rv, utc1, utc2, dut1,
                     elong, phi, hm, xp, yp, phpa, tc, rh, wl,
                     &aob, &zob, &hob, &dob, &rob, &eo ) ) return -1;
    reprd ( "ICRS -> observed:", rob, dob );
    return 0;
}
#include <stdio.h>
void reprd ( char* s, double ra, double dc )
{
    char pm;
    int i[4];
    printf ( "%25s", s );
    iauA2tf ( 7, ra, &pm, i );
    printf ( " %2.2d %2.2d %2.2d.%7.7d", i[0],i[1],i[2],i[3] );
    iauA2af ( 6, dc, &pm, i );
    printf ( " %c%2.2d %2.2d %2.2d.%6.6d\n", pm, i[0],i[1],i[2],i[3] );
}