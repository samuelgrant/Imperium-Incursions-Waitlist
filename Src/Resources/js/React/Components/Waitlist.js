import React, { Component } from 'react';
import { XmppLink } from './CommLinks';
import { Pilot, Destination } from './EsiUi';
import ReactTooltip from 'react-tooltip'
import { setTimeout } from 'timers';

export default class Waitlist extends Component {


    render() {
        let FlightStrips;
        if (this.props.waitlist) {

            FlightStrips = this.props.waitlist.map((pilot) => {
                return <FlightStrip pilot={pilot} wings={this.props.wings} fleetId={this.props.fleetId} />;
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
                            <th></th>
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
    alarmAccount(wl_id, account_id) {
        $(`#${wl_id}`).addClass('tr-pending');

        $.ajax({
            type: 'post',
            url: `/fleets/${this.props.fleetId}/alarm/${account_id}`
        }).done((data) => {
            // YAY
        }).fail((err) => {
            $(`#${wl_id}`).removeClass('tr-pending').addClass('tr-danger');
            console.error(`React/Components/Waitlist {FlightStrip@alarmAccount} - Error sending the pilot an alarm`, err.responseText);
        })
    }

    invitePilot(wl_id, pilot_id, squad_id, wing_id) {
        $(`#${wl_id}`).addClass("tr-pending");

        $.ajax({
            type: 'post',
            url: `/fleets/${this.props.fleetId}/invite/${pilot_id}`,
            data: { squadId: squad_id, wingId: wing_id }
        }).done((data) => {
            $(`#${wl_id}`).addClass("tr-success");
        }).fail((err) => {
            $(`#${wl_id}`).removeClass('tr-pending').addClass('tr-danger');
            console.error(`React/Components/Waitlist {FlightStrip@invitePilot} - Error inviting the pilot ${pilot_id}`, err.responseText);
        });
    }

    removePilot(wl_id) {
        $.ajax({
            type: 'delete',
            url: `/waitlist/remove/${wl_id}`
        }).done((data) => {
            $(`#${wl_id}`).addClass('tr-danger');
        }).fail((err) => {
            console.error(`React/Components/Waitlist {FlightStrip@removePilot} - Error removing the pilot from the waitlist`, err.responseText);
        });
    }

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
                return <button className="btn btn-outline-success btn-sm m-1" data-tip={role.name}>{role.acronym.charAt(0)}</button>;
            });
        }

        let squads;
        if (this.props.wings) {
            squads = this.props.wings.map((wing) => {
                return wing.squads.map((squad) => {
                    return <a role="presentation" className="dropdown-item" onClick={this.invitePilot.bind(this, this.props.pilot.account.id, this.props.pilot.pilot.characterID, squad.id, wing.id)}>{`${wing.name}: ${squad.name}`}</a>
                });
            });
        }

        return (
            <tr id={this.props.pilot.id}>
                <td> <img src={`https://image.eveonline.com/Character/${this.props.pilot.pilot.characterID}_64.jpg`} height="50" /> </td>
                <td>
                    <Pilot pilot={this.props.pilot.pilot} />
                    <div className="clearfix" />
                    <XmppLink AuthName={this.props.pilot.account.name} />
                    <div className="clearfix" />
                    {newPilot} {offline}
                </td>
                <td>
                    <div role="group" className="btn-group btn-group-sm p-1">
                        <button className="btn btn-success btn-sm" type="button" onClick={this.invitePilot.bind(this, this.props.pilot.account.id, this.props.pilot.pilot.characterID, null, null)}>Invite <i className="fas fa-plus"></i></button>

                        <div className="dropdown btn-group d-inline" role="group">
                            <button className="btn btn-success btn-sm dropdown-toggle" data-toggle="dropdown" aria-expanded="false" type="button"></button>
                            <div role="menu" className="dropdown-menu">
                                {squads}
                            </div>
                        </div>

                    </div>

                    <button className="btn btn-danger btn-sm p-1" type="button" onClick={this.removePilot.bind(this, this.props.pilot.id)}><i className="fas fa-minus"></i></button>

                    <div className="dropdown d-inline p-1">
                        <button className="btn btn-dark btn-sm dropdown-toggle" data-toggle="dropdown" aria-expanded="false" type="button">Options <i className="fas fa-cog"></i> </button>
                        <div role="menu" className="dropdown-menu">
                            <a role="presentation" className="dropdown-item" href="/search?q=pilot_name&action=viewProfile">Pilot Profile</a>
                            <a role="presentation" class="dropdown-item" href={`xmpp:${this.props.pilot.account.name.toLowerCase().replace(/ /g, "_")}`}>Open Jabber PM</a>
                        </div>
                    </div>

                    <button className="btn btn-warning btn-sm p-1 disabled" type="button" onClick={this.alarmAccount.bind(this, this.props.pilot.id, this.props.pilot.account.accountId)}><i className="fas fa-bell"></i></button>
                </td>
                <td>
                    {fits}
                </td>
                <td> {roles} </td>
                <td> <Destination system={this.props.pilot.system} /> </td>
                <td> {this.props.pilot.waitingFor} </td>
                <ReactTooltip />
            </tr>
        );
    }
}