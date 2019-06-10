import React, { Component } from 'react';
import Modal from './modal';
import { Input } from './FormControls';

export class NewFleetLink extends Component {
    render() {
        return (
            <div className="col-4">
                <button className="btn btn-dark d-block mx-auto" data-toggle="modal" data-target="#newFleetModal">Register a New Fleet</button>
            </div>
        )
    }
}

export class NewFleetModal extends Component {
    registerFleet(e) {
        e.preventDefault();

        $.ajax({
            type: 'post',
            url: `/fleets`,
            data: $("#newFleetForm").serialize()
        }).done((fleetId) => {
            window.location.href = `/fleets/${fleetId}`
        }).fail((err) => {
            console.error(`React/NewFleetModal {NewFleetModal@registerFleet} - Error registering the fleet`, err.responseText);
        })
    }

    render() {

        let boss;
        if (this.props.options) {
            boss = this.props.options.pilots.map((pilot) => {
                let selected = false;

                if (this.props.prefPilot && this.props.prefPilot.id == pilot.id)
                    selected = true;

                return <option value={pilot.id} selected={selected}>{pilot.name}</option>
            });
        }

        let types;
        if (this.props.options) {
            types = this.props.options.fleetTypes.map((type) => {
                return <option value={type}>{type}</option>
            });
        }

        let comms;
        if (this.props.options) {
            comms = this.props.options.comms.map((channel) => {
                return <option value={channel.id}>{channel.linkText}</option>
            });
        }

        return (
            <Modal id="newFleetModal" title="Start a fleet!" dismiss="true">
                <form id="newFleetForm" onSubmit={this.registerFleet.bind(this)}>
                    <div className="form-group">
                        <label className="required" htmlFor="EsiFleet">ESI Fleet URL:</label>
                        <Input id="EsiFleet" name="EsiFleetUrl" placeholder="https://esi.evetech.net/v1/fleets/.../?datasource=tranquility" autocomplete="false" required="true"/>
                    </div>

                    <div className="form-group">
                        <label className="required" htmlFor="FleetBoss">Fleet Boss:</label>
                        <select id="FleetBoss" name="FleetBoss" className="form-control" required>
                            {boss}
                        </select>
                        <span className="text-muted font-italic">Must be the pilot with the star in game.</span>
                    </div>

                    <div className="form-group">
                        <label className="required" htmlFor="FleetType">Fleet Boss:</label>
                        <select id="FleetType" name="FleetType" className="form-control" required>
                            {types}
                        </select>
                        <span className="text-muted font-italic">This will display your fleet cap to pilots.</span>
                    </div>

                    <div className="form-group">
                        <label className="required" htmlFor="FleetComms">Fleet Comms:</label>
                        <select id="FleetComms" name="FleetComms" className="form-control" required>
                            {comms}
                        </select>
                    </div>

                    <button className="btn btn-success pull-right">Register Fleet</button>
                </form>
            </Modal>
        )
    }
}