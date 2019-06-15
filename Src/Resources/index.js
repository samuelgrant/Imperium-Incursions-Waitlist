import _ from 'lodash';


/* JavaScript Core */
import './js/Libraries/popper.min';
//import './js/Bootstrap/index';

/* React */
import './js/React/Announcement';
import './js/React/Bans';
import './js/React/Commanders';
import './js/React/Index';
import './js/React/FleetManagement';
import './js/React/FittingsAndSkills';
import './js/React/SystemSettings';

/* REACT -> Pilot Select Page */
import './js/React/PilotSelect';

/* REACT -> Navbar */
import './js/React/TqClock';
import './js/React/TqStatus';

/* Theme */
import './js/app.min';
import './js/Libraries/Theme/app';
import './js/Libraries/Theme/vendors';

// Sidebar
$('.sidebarBtn').click(function () {
    $('.sidebar-special').toggleClass('active');
    $('.sidebarBtn').toggleClass('toggle');
});
