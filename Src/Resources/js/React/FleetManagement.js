import React, { Component } from 'react';
import { render } from 'react-dom';

const baseUri = "/fleets";

export default class Index extends Component {
    constructor(props) {
        super(props);

        this.state = {
            fleets: null,
            fcOptions: null
        }
    }


    hideNav() {
        $('.sidebar-special').removeClass('active');
    }

    openNav(id) {
        this.hideNav();

        $(`#${id}`).addClass('active');
    }

    render() {
        return (
            <div className="container">
                <button className="btn btn-dark" onClick={this.openNav.bind(this, "fleetCynos")}>Fleet Cynos</button>
                <button className="btn btn-dark" onClick={this.openNav.bind(this, "fleetSettings")}>Fleet Settings</button>

                <div id="fleetSettings" className="sidebar-special">
                    <h3>
                        Fleet Settings
                        <i className="fas fa-chevron-double-right float-right" onClick={this.hideNav.bind(this)}></i>
                    </h3>
                    
                </div>

                <div id="fleetCynos" className="sidebar-special">
                    <h3>
                        Fleet Cynos
                        <i className="fas fa-chevron-double-right float-right" onClick={this.hideNav.bind(this)}></i>
                    </h3>

                </div>
            </div>
        )
    }
}



if (document.getElementById('fleetManagement'))
    render(<Index />, document.getElementById('fleetManagement'));