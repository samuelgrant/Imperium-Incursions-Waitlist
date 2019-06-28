import React, { Component } from 'react';
import { Pilot, Destination } from '../EsiUi';
import { MumbleLink } from '../CommLinks';

export default class FleetInfo extends Component {

    joinFleet(id) {
        $.ajax({
            type: 'post',
            url: `/fleets/${this.props.fleet.id}/invite/${id}`,
        }).fail((err) => {
            console.error(`React/Components/FleetInfo {FleetInfo@joinFleet} - Error joining the waitlist`, err.responseText);
        }) 
    }

    render() {
        let myPilots;
        if (this.props.myPilots) {
            myPilots = this.props.myPilots.map((pilot) => {
                return <a role="presentation" className="dropdown-item" onClick={this.joinFleet.bind(this, pilot.id)}>{pilot.name}</a>;
            });
        }

        let fcButtons;
        if (this.props.showFcOptions) {
            fcButtons = (
                <div role="group" className="btn-group special">
                    <a className="btn btn-dark" type="button" href={`/fleets/${this.props.fleet.id}`}>Manage Fleet</a>

                    <div className="dropdown btn-group" role="group">
                        <button className="btn btn-success dropdown-toggle" data-toggle="dropdown" aria-expanded="false" type="button">Join Fleet With....</button>
                        <div role="menu" className="dropdown-menu">
                            {myPilots}
                        </div>
                    </div>
                </div>
            )
        }

        return (
            <div className="col-lg-4 col-md-6 col-sm-12">
                <div className="fleet-info">
                    <div className="row">
                        <div className="col-lg-6 col-md-12">
                            <FleetFC fc={this.props.fleet.fc} />
                        </div>

                        <div className="col-lg-6 col-md-12">
                            <FleetLocation system={this.props.fleet.system} />
                        </div>
                    </div>
                        
                    <div className="row">
                        <div className="col-lg-6 col-md-12">
                            <FleetCount members={this.props.fleet.members} />
                        </div>

                        <div className="col-lg-6 col-md-12">
                            <MumbleLink commChannel={this.props.fleet.comms} />
                        </div>
                    </div>

                    {fcButtons}
                </div>
            </div>
        )
    }
}

class FleetFC extends Component {
    render() {
        return (
            <span>
                FC: <Pilot pilot={this.props.fc} />
            </span>
        )
    }
}

class FleetLocation extends Component {
    render() {
        return (
            <span>
                <i className="fas fa-map-marker-alt"></i>
                <Destination system={this.props.system} />
            </span>
        )
    }
}

class FleetCount extends Component {
    render() {
        return (
            <span>
                <i className="fas fa-users"></i>
                {`${this.props.members.onGrid} / ${this.props.members.max}`}
            </span>
        )
    }
}