import React, { Component } from 'react';
import { XmppLink } from './CommLinks';
import { Pilot, Destination } from './EsiUi';
import ReactTooltip from 'react-tooltip'

export default class Waitlist extends Component {


    render() {
        let FlightStrips;
        if (this.props.waitlist) {

            FlightStrips = this.props.waitlist.map((pilot) => {
                return <FlightStrip pilot={pilot} />;
            });
        }

        return (
            <div className="table-responsive">
                <table className="table">
                    <thead>
                        <tr>
                            <th style={{ "width": "30px" }}></th>
                            <th>Pilot</th>
                            <th style={{ "width": "220px" }}></th>
                            <th style={{ "width": "180px" }}>Ships</th>
                            <th>Roles</th>
                            <th>System</th>
                            <th>Wait Time</th>
                        </tr>
                    </thead>
                    <tbody>
                        {FlightStrips}
                    </tbody>
                </table>
            </div>
        )
    }
}

export class FlightStrip extends Component {
    render() {
        let newPilot;
        if (this.props.pilot.newPilot) {
            newPilot = <span className="text-danger fa-status"><i class="fas fa-exclamation-triangle"></i> New Pilot</span>
        }

        let offline;
        if (this.props.pilot.isOffline) {
            offline = <span className="text-warning fa-status"><i class="fas fa-user-slash"></i> Offline</span>
        }

        let fits;
        if (this.props.pilot.ships) {
            fits = this.props.pilot.ships.map((ship) => {
                return <img className="m-1" data-tip={ship.description} src={`https://image.eveonline.com/Render/${ship.shipTypeId}_32.png`} />;
            });
        }

        let roles;
        if (this.props.pilot.roles) {
            roles = this.props.pilot.roles.map((role) => {
                return <button className="btn btn-outline-success btn-sm m-1 active" data-tip={role.name}>{role.acronym.charAt(0)}</button>;
            });
        }

        return (
            <tr>
                <ReactTooltip />
                <td>
                    <img src={`https://image.eveonline.com/Character/${this.props.pilot.pilot.characterID}_64.jpg`} height="50" />
                </td>
                <td>
                    <Pilot pilot={this.props.pilot.pilot} />
                    <div className="clearfix" />
                    <XmppLink AuthName={this.props.pilot.account.name} />
                    <div className="clearfix" />
                    {newPilot} {offline}
                </td>
                <td>
                    <div role="group" className="btn-group btn-group-sm p-1">
                        <button className="btn btn-success btn-sm disabled" type="button">Invite <i className="fas fa-plus"></i></button>
                        <div className="dropdown btn-group d-inline" role="group">
                            <button className="btn btn-success btn-sm dropdown-toggle disabled" data-toggle="dropdown" aria-expanded="false" type="button"></button>
                            <div role="menu" className="dropdown-menu">
                                <a role="presentation" className="dropdown-item" href="#">Squad One</a>
                            </div>
                        </div>
                    </div>
                    <button className="btn btn-danger btn-sm p-1 disabled" type="button"><i className="fas fa-minus"></i></button>
                    <div className="dropdown d-inline p-1">
                        <button className="btn btn-dark btn-sm dropdown-toggle disabled" data-toggle="dropdown" aria-expanded="false" type="button">Options <i className="fas fa-cog"></i> </button>
                        <div role="menu" className="dropdown-menu">
                            <a role="presentation" className="dropdown-item" href="#">Pilot Profile</a>
                            <a role="presentation" class="dropdown-item" href="#">Open Jabber PM</a>
                        </div>
                    </div>
                    <button className="btn btn-warning btn-sm p-1 disabled" type="button"><i className="fas fa-bell"></i></button>
                </td>
                <td>
                    {fits}
                </td>
                <td>
                    {roles}
                </td>
                <td><Destination system={this.props.pilot.system} /></td>
                <td>{this.props.pilot.waitingFor}</td>
            </tr>
        );
    }
}