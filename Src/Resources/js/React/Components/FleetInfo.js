import React, { Component } from 'react';
import { Pilot, Destination } from './EsiUi';
import { MumbleLink } from './CommLinks';

export default class FleetInfo extends Component {

    render() {
        let myPilots;
        if (this.props.myPilots) {
            myPilots = this.props.myPilots.map((pilot) => {
                return <a role="presentation" className="dropdown-item" href="#">{pilot.name}</a>;
            });
        }
        return (
            <div className="col-lg-4 col-md-6 col-sm-12">
                <div className="card fleet-info">
                    <div className="card-body">
                        <div className="row">
                            <div className="col-lg-6 col-md-12">
                                FC: <Pilot pilot={this.props.fleet.fc}/>
                            </div>

                            <div className="col-lg-6 col-md-12">
                                <Destination system={this.props.fleet.system} />
                            </div>
                        </div>
                        
                        <div className="row">
                            <div className="col-lg-6 col-md-12">
                                <i className="fas fa-users"></i> 
                                {this.props.fleet.memberCount}
                            </div>

                            <div className="col-lg-6 col-md-12">
                                <MumbleLink commChannel={this.props.fleet.comms} />
                            </div>
                        </div>


                        <div role="group" className="btn-group special">
                            <a className="btn btn-dark" type="button" href={`/fleets/${this.props.fleet.id}`}>Manage Fleet</a>

                            <div className="dropdown btn-group" role="group">
                                <button className="btn btn-success dropdown-toggle disabled" data-toggle="dropdown" aria-expanded="false" type="button">Join Fleet With....</button>
                                <div role="menu" className="dropdown-menu">
                                    {myPilots}
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        )
    }
}